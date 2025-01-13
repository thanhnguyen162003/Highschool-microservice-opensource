using Application.Common.Models.FlashcardFeatureModel;
using Application.Common.Models.FlashcardTestModel;
using Application.Common.Ultils;
using Application.Constants;
using Application.Features.StudyFlashcardFeature.Commands;
using Application.Features.StudyFlashcardFeature.Queries;
using Application.Features.TestFlashcardFeature.Commands;
using Application.Features.TestFlashcardFeature.Queries;
using Carter;
using Domain.CustomModel;
using Microsoft.AspNetCore.Mvc;


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
			group.MapDelete("flashcard/{id}/reset-progress", ResetProgress).RequireAuthorization().WithName(nameof(ResetProgress));
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
			return JsonHelper.Json(result);
		}
		public static async Task<IResult> ResetProgress(Guid id, ISender sender, CancellationToken cancellationToken)
		{
			var command = new ResetProgressCommand()
			{
				FlashcardId = id
			};
			var result = await sender.Send(command, cancellationToken);
			return JsonHelper.Json(result);
		}
	}
}