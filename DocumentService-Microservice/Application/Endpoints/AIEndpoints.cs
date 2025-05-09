using Application.Common.Interfaces.AIInferface;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Application.Common.Models.FlaschardStreamModel;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.Features.FlashcardContentFeature.Commands;
using Carter;
using Domain.DraftContent;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;

namespace Application.Endpoints
{
	public class AIEndpoints : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			var group = app.MapGroup("api/v1/beta");
			group.MapPost("ai-flashcard", GenerateFlashcard).RequireAuthorization().WithName(nameof(GenerateFlashcard)).DisableAntiforgery();

			// Streaming endpoint
			group.MapPost("ai-flashcard/initialize", InitializeFlashcardStream).RequireAuthorization().WithName(nameof(InitializeFlashcardStream)).DisableAntiforgery();
			group.MapGet("ai-flashcard/stream/{id}", StreamFlashcards).RequireAuthorization().WithName(nameof(StreamFlashcards));
		}

		public static async Task<IResult> GenerateFlashcard(ISender sender, CancellationToken cancellationToken,
			[FromForm] AIFlashcardRequestModel aIFlashcardRequestModel, ValidationHelper<AIFlashcardRequestModel> validationHelper)
		{
			var (isValid, response) = await validationHelper.ValidateAsync(aIFlashcardRequestModel);
			if (!isValid)
			{
				return Results.BadRequest(response);
			}
			var command = new FlashcardAIGeneratorCommand()
			{
				AIFlashcardRequestModel = aIFlashcardRequestModel
			};
			var result = await sender.Send(command, cancellationToken);

			return JsonHelper.Json(result);
		}
		// Step 1: Initialize streaming session
		public static async Task<IResult> InitializeFlashcardStream(ISender sender, CancellationToken cancellationToken,
			[FromForm] AIFlashcardRequestModel aIFlashcardRequestModel, ValidationHelper<AIFlashcardRequestModel> validationHelper,
			IClaimInterface claim, IUnitOfWork unitOfWork, ILogger<AIEndpoints> logger)
		{
			var (isValid, response) = await validationHelper.ValidateAsync(aIFlashcardRequestModel);
			if (!isValid)
			{
				return Results.BadRequest(response);
			}

			var userId = claim.GetCurrentUserId;
			var numberUserFlashcard = await unitOfWork.FlashcardRepository.CheckNumberFlashcardInUser(userId);
			if (numberUserFlashcard >= 20)
			{
				return Results.BadRequest(new ResponseModel(HttpStatusCode.BadRequest, "Bạn chỉ có thể tối đa 20 thẻ ghi nhớ"));
			}

			var flashcardDraftCheck = await unitOfWork.FlashcardRepository.GetFlashcardDraftAIByUserId(userId);
			if (flashcardDraftCheck is not null)
			{
				return Results.BadRequest(new ResponseModel(HttpStatusCode.BadRequest, "Bạn có bản nháp của thẻ ghi nhớ", flashcardDraftCheck.Id));
			}

			if (aIFlashcardRequestModel.FileRaw == null && string.IsNullOrWhiteSpace(aIFlashcardRequestModel.TextRaw))
			{
				return Results.BadRequest(new ResponseModel(HttpStatusCode.BadRequest, "Cần có file hoặc text để phân tích."));
			}

			// Create the flashcard entry in database
			var newId = new UuidV7().Value;
			Flashcard flashcard = new Flashcard()
			{
				Id = newId,
				UserId = userId,
				FlashcardName = FlashcardCreateDraftContent.TitleAI,
				FlashcardDescription = FlashcardCreateDraftContent.DescriptionAI,
				Status = StatusConstant.ONLYLINK,
				Created = false,
				UpdatedAt = DateTime.UtcNow,
				CreatedBy = userId.ToString(),
				Slug = SlugHelper.GenerateSlug(FlashcardCreateDraftContent.Title, newId.ToString()),
				IsArtificalIntelligence = true,
				FlashcardType = Domain.Enums.FlashcardType.Lesson,
				IsCreatedBySystem = false
			};

			await unitOfWork.FlashcardRepository.CreateFlashcard(flashcard);

			// Store request in cache for streaming processing
			var cacheKey = $"flashcard-stream:{newId}";
			FlashcardStreamingState state = new FlashcardStreamingState
			{
				FlashcardId = newId,
				UserId = userId,
				RequestModel = aIFlashcardRequestModel,
				Status = StreamingStatus.Initialized
			};

			StreamingStateManager.AddOrUpdateState(cacheKey, state);

			
			logger?.LogInformation("Starting background task for flashcardId: {FlashcardId}", newId);

			// Initiate background processing
			_ = Task.Run(() =>
			{
				logger?.LogInformation("Inside Task.Run for flashcardId: {FlashcardId}", newId);
				return ProcessFlashcardGenerationInBackground(newId, state, cancellationToken);
			});

			logger?.LogInformation("Task.Run initiated for flashcardId: {FlashcardId}", newId);

			return Results.Ok(new ResponseModel(HttpStatusCode.OK, "Bắt đầu tạo thẻ ghi nhớ.", newId));
		}

		// Step 2: Stream the results
		public static async Task StreamFlashcards(HttpContext context, string id)
		{
			var cacheKey = $"flashcard-stream:{id}";
			var state = StreamingStateManager.GetState(cacheKey);

			if (state == null)
			{
				await context.Response.WriteAsync("data: " + JsonConvert.SerializeObject(new { error = "Session not found" }) + "\n\n");
				return;
			}

			context.Response.Headers.Add("Content-Type", "text/event-stream");
			context.Response.Headers.Add("Cache-Control", "no-cache");
			context.Response.Headers.Add("Connection", "keep-alive");

			// Send initial state
			await context.Response.WriteAsync("data: " + JsonConvert.SerializeObject(new
			{
				status = state.Status.ToString(),
				totalItems = state.TotalItems,
				processedItems = state.ProcessedItems
			}) + "\n\n");
			await context.Response.Body.FlushAsync();

			// Register for updates and stream them
			using var subscription = state.Updates.Subscribe(async update =>
			{
				if (update is FlashcardContentCreateRequestModel flashcard)
				{
					await context.Response.WriteAsync("data: " + JsonConvert.SerializeObject(flashcard) + "\n\n");
				}
				else if (update is StatusUpdate statusUpdate)
				{
					await context.Response.WriteAsync("data: " + JsonConvert.SerializeObject(new
					{
						status = statusUpdate.Status.ToString(),
						totalItems = statusUpdate.TotalItems,
						processedItems = statusUpdate.ProcessedItems,
						error = statusUpdate.Error
					}) + "\n\n");

					// If complete or error, end the stream
					if (statusUpdate.Status == StreamingStatus.Completed ||
						statusUpdate.Status == StreamingStatus.Failed)
					{
						await context.Response.WriteAsync("data: [DONE]\n\n");
					}
				}

				await context.Response.Body.FlushAsync();
			});

			// Keep connection open until client disconnects or processing completes
			var tcs = new TaskCompletionSource<bool>();
			context.RequestAborted.Register(() => tcs.TrySetResult(true));

			// Also complete if state is already done
			if (state.Status == StreamingStatus.Completed || state.Status == StreamingStatus.Failed)
			{
				tcs.TrySetResult(true);
			}

			await tcs.Task;
		}

		// Background processing method
		private static async Task ProcessFlashcardGenerationInBackground(
			Guid flashcardId,
			FlashcardStreamingState state,
			CancellationToken cancellationToken)
		{
			try
			{
				using var scope = StreamingStateManager.ServiceScopeFactory.CreateScope();
				var aiService = scope.ServiceProvider.GetRequiredService<IAIService>();
				var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
				var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
				var logger = scope.ServiceProvider.GetRequiredService<ILogger<AIEndpoints>>();

				state.Status = StreamingStatus.Processing;
				state.Updates.OnNext(new StatusUpdate
				{
					Status = StreamingStatus.Processing
				});

				var flashcards = new List<FlashcardContentCreateRequestModel>();

				await foreach (var flashcard in aiService.GenerateFlashcardContentStreamAsync(
					state.RequestModel.Note,
					state.RequestModel.FileRaw,
					state.RequestModel.TextRaw,
					state.RequestModel.NumberFlashcardContent,
					state.RequestModel.LevelHard,
					state.RequestModel.FrontTextLong,
					state.RequestModel.BackTextLong,
					cancellationToken))
				{
					flashcards.Add(flashcard);

					// Notify subscribers
					state.ProcessedItems++;
					state.Updates.OnNext(flashcard);

					// Save to database in real-time
					var flashcardContent = mapper.Map<FlashcardContent>(flashcard);
					flashcardContent.Id = new UuidV7().Value;
					flashcardContent.FlashcardId = flashcardId;
					flashcardContent.CreatedAt = DateTime.UtcNow;
					flashcardContent.CreatedBy = state.UserId.ToString();
					flashcardContent.UpdatedBy = state.UserId.ToString();
					flashcardContent.Status = StatusConstant.ONLYLINK;

					await unitOfWork.FlashcardContentRepository.CreateFlashcardContentSingle(flashcardContent);
				}

				// Update status to completed
				state.Status = StreamingStatus.Completed;
				state.TotalItems = flashcards.Count;
				state.Updates.OnNext(new StatusUpdate
				{
					Status = StreamingStatus.Completed,
					TotalItems = flashcards.Count,
					ProcessedItems = flashcards.Count
				});

				_ = Task.Delay(TimeSpan.FromMinutes(5))
					.ContinueWith(_ => StreamingStateManager.RemoveState($"flashcard-stream:{flashcardId}"));
			}
			catch (Exception ex)
			{
				state.Status = StreamingStatus.Failed;
				state.Updates.OnNext(new StatusUpdate
				{
					Status = StreamingStatus.Failed,
					Error = ex.Message
				});

				_ = Task.Delay(TimeSpan.FromMinutes(5))
					.ContinueWith(_ => StreamingStateManager.RemoveState($"flashcard-stream:{flashcardId}"));
			}
		}
		public static class StreamingStateManager
		{
			private static readonly ConcurrentDictionary<string, FlashcardStreamingState> _states = new();
			public static IServiceScopeFactory ServiceScopeFactory { get; set; }

			public static FlashcardStreamingState GetState(string key)
			{
				_states.TryGetValue(key, out var state);
				return state;
			}

			public static void AddOrUpdateState(string key, FlashcardStreamingState state)
			{
				_states.AddOrUpdate(key, state, (_, _) => state);
			}

			public static void RemoveState(string key)
			{
				_states.TryRemove(key, out _);
			}
		}

	}
}

