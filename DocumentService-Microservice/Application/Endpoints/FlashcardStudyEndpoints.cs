using Application.Common.Models.FlashcardFeatureModel;
using Application.Common.Models.FlashcardTestModel;
using Application.Common.Ultils;
using Application.Constants;
using Application.Features.FlashcardContentFeature.Commands;
using Application.Features.StudyFlashcardFeature.Commands;
using Application.Features.StudyFlashcardFeature.Queries;
using Application.Features.TestFlashcardFeature.Commands;
using Application.Features.TestFlashcardFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Application.Endpoints
{
	public class FlashcardStudyEndpoints : ICarterModule
	{
		public void AddRoutes(IEndpointRouteBuilder app)
		{
			var group = app.MapGroup("api/v1/feature");
			group.MapGet("flashcard/{id}/study", GetFlashcardStudyGenerate).RequireAuthorization().WithName(nameof(GetFlashcardStudyGenerate));
			group.MapGet("flashcard/{id}/test", GetFlashcardTestGenerate).RequireAuthorization().WithName(nameof(GetFlashcardTestGenerate));
			group.MapPost("flashcard/{id}/submit-test-multiple", SubmitTest).RequireAuthorization().WithName(nameof(SubmitTest));
			group.MapPost("flashcard/{id}/submit-test-true-false", SubmitTestTrueFalse).RequireAuthorization().WithName(nameof(SubmitTestTrueFalse));
			group.MapGet("flashcard/{id}/progress", GetFlashcardUserProgress).RequireAuthorization().WithName(nameof(GetFlashcardUserProgress));
			group.MapPost("flashcard/progress", UpdateProgress).RequireAuthorization().WithName(nameof(UpdateProgress));
			group.MapPost("flashcard/batch-progress", BatchUpdateProgress).RequireAuthorization().WithName(nameof(BatchUpdateProgress));
			group.MapDelete("flashcard/{id}/reset-progress", ResetProgress).RequireAuthorization().WithName(nameof(ResetProgress));
            group.MapDelete("flashcardContent/{flashcardContentId}/reset-progress", ResetFlashcardContentProgress).RequireAuthorization().WithName(nameof(ResetFlashcardContentProgress));

            // Thêm các endpoint mới cho FSRS
            group.MapGet("flashcard/{id}/fsrs-detail", GetFSRSDetailProgress).RequireAuthorization().WithName(nameof(GetFSRSDetailProgress));
			group.MapGet("flashcard/{id}/learn", LearnFlashcard).RequireAuthorization().WithName(nameof(LearnFlashcard));
            group.MapGet("flashcard/{slug}/learn-slug", LearnFlashcardSlug).RequireAuthorization().WithName(nameof(LearnFlashcardSlug));
            group.MapPost("flashcardContent/{flashcardContentId}/assess-answer", AssessAnswer).RequireAuthorization().WithName(nameof(AssessAnswer));
            group.MapGet("flashcard/{slug}/remembered", GetRememberedFlashcards).RequireAuthorization().WithName(nameof(GetRememberedFlashcards));

            //group.MapGet("learning-analytics", GetLearningAnalytics).RequireAuthorization().WithName(nameof(GetLearningAnalytics));
        }

		public static async Task<IResult> GetFlashcardStudyGenerate(Guid id, ISender sender, CancellationToken cancellationToken)
		{
			var query = new FlashcardDetailStudyQuery()
			{
				FlashcardId = id
			};
			var result = await sender.Send(query, cancellationToken);
			return JsonHelper.Json(result);
		}
		public static async Task<IResult> GetFlashcardTestGenerate(Guid id, [AsParameters] TestOptionModel optionModel, ISender sender, CancellationToken cancellationToken)
		{
			object query;
			if (optionModel.TypeTest.Equals(TypeTestConstraint.MULTIPLECHOICE))
			{
				query = new FlashcardTestQuery()
				{
					FlashcardId = id,
					TestOptionModel = optionModel
				};
			}
			else if (optionModel.TypeTest.Equals(TypeTestConstraint.TRUEFALSE))
			{
				query = new FlashcardTestTrueFalseQuery()
				{
					FlashcardId = id,
					TestOptionModel = optionModel
				};
			}
			else
			{
				query = new FlashcardTestQuery()
				{
					FlashcardId = id,
					TestOptionModel = optionModel
				};
			}
			var result = await sender.Send(query, cancellationToken);
			return JsonHelper.Json(result);
		}
		public static async Task<IResult> SubmitTest(Guid id, [FromBody] List<FlashcardAnswerSubmissionModel> flashcardAnswerSubmissionModel
			, ISender sender, CancellationToken cancellationToken)
		{
			var query = new SubmitAnswerCommand()
			{
				FlashcardId = id,
				FlashcardAnswerSubmissionModel = flashcardAnswerSubmissionModel
			};
			var result = await sender.Send(query, cancellationToken);
			return JsonHelper.Json(result);
		}
		
		public static async Task<IResult> SubmitTestTrueFalse(Guid id, [FromBody] List<TrueFalseAnswerSubmissionModel> trueFalseAnswerSubmissionModel
			, ISender sender, CancellationToken cancellationToken)
		{
			var query = new SubmitAnswerTrueFalseCommand()
			{
				FlashcardId = id,
				TrueFalseAnswerSubmissionModel = trueFalseAnswerSubmissionModel
			};
			var result = await sender.Send(query, cancellationToken);
			return JsonHelper.Json(result);
		}
		
		public static async Task<IResult> GetFlashcardUserProgress(Guid id, ISender sender, CancellationToken cancellationToken)
		{
			var command = new FlashcardProgressStudyQuery()
			{
				FlashcardId = id
			};
			var result = await sender.Send(command, cancellationToken);
			return JsonHelper.Json(result);
		}
		public static async Task<IResult> UpdateProgress([FromBody] UpdateProgressModel updateProgressModel, ISender sender, CancellationToken cancellationToken)
		{
			var command = new UpdateUserProgressStudyCommand()
			{
				UpdateProgressModel = updateProgressModel
			};
			var result = await sender.Send(command, cancellationToken);
			return Results.Json(result, statusCode: (int)result.Status);
		}
		public static async Task<IResult> ResetProgress(Guid id, ISender sender, CancellationToken cancellationToken)
		{
			var command = new ResetProgressCommand()
			{
				FlashcardId = id
			};
			var result = await sender.Send(command, cancellationToken);
			return Results.Json(result, statusCode: (int)result.Status);
		}
		
		// Thêm các phương thức mới cho FSRS
		
		public static async Task<IResult> GetFSRSDetailProgress(Guid id, ISender sender, CancellationToken cancellationToken)
		{
			var query = new FSRSDetailProgressQuery()
			{
				FlashcardId = id
			};
			var result = await sender.Send(query, cancellationToken);
			return JsonHelper.Json(result);
		}
		
		public static async Task<IResult> LearnFlashcard(
			Guid id, 
			ISender sender, 
			CancellationToken cancellationToken, 
			HttpContext httpContext,
			[FromQuery] bool isLearningNew = false)
		{
			var query = new DueFlashcardsQuery()
			{
				FlashcardId = id,
				IsLearningNew = isLearningNew,
			};
			var result = await sender.Send(query, cancellationToken);
						
			return JsonHelper.Json(result);
		}

        public static async Task<IResult> LearnFlashcardSlug(
            [FromRoute] string slug,
            ISender sender,
            CancellationToken cancellationToken,
            HttpContext httpContext,
            [FromQuery] bool isLearningNew = false)
        {
            var query = new DueFlashcardsQuery()
            {
                FlashcardSlug = slug,
                IsLearningNew = isLearningNew,
            };
            var result = await sender.Send(query, cancellationToken);

            return JsonHelper.Json(result);
        }

        public static async Task<IResult> GetLearningAnalytics(
            ISender sender, CancellationToken cancellationToken,

            [FromQuery] DateTime? startDate = null,
			[FromQuery] DateTime? endDate = null
			)
		{
			var query = new LearningAnalyticsQuery()
			{
				StartDate = startDate,
				EndDate = endDate
			};
			var result = await sender.Send(query, cancellationToken);
			return JsonHelper.Json(result);
		}
		
		public static async Task<IResult> BatchUpdateProgress([FromBody] BatchUpdateProgressModel batchUpdateProgressModel, ISender sender, CancellationToken cancellationToken)
		{
			var command = new BatchUpdateUserProgressCommand()
			{
				BatchProgressUpdates = batchUpdateProgressModel
			};
			var result = await sender.Send(command, cancellationToken);
			return Results.Json(result, statusCode: (int)result.Status);
		}

        public static async Task<IResult> AssessAnswer([FromRoute] Guid flashcardContentId, [FromBody] string userAnswer
            , ISender sender, CancellationToken cancellationToken)
        {
            var query = new AIAssessCommand()
            {
                FlashcardContentId = flashcardContentId,
                UserAnswer = userAnswer
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }

        public static async Task<IResult> ResetFlashcardContentProgress([FromRoute] Guid flashcardContentId, ISender sender, CancellationToken cancellationToken)
        {
            var command = new ResetFlashcardContentProgressCommand()
            {
                FlashcardContentId = flashcardContentId
            };
            var result = await sender.Send(command, cancellationToken);
            return Results.Json(result, statusCode: (int)result.Status);
        }

        public static async Task<IResult> GetRememberedFlashcards(
	        [FromRoute] string slug,
			ISender sender,
			CancellationToken cancellationToken,
			[FromQuery] RememberedFlashcardsMode mode = RememberedFlashcardsMode.Today,
			[FromQuery] int limit = 100)
        {
            var query = new RememberedFlashcardsQuery()
            {
                Slug = slug,
                Mode = mode,
                Limit = limit
            };
            var result = await sender.Send(query, cancellationToken);
            return JsonHelper.Json(result);
        }
    }
}