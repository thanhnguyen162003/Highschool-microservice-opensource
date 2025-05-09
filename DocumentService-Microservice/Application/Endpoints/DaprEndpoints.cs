using Application.Common.Models.DaprModel.Document;
using Application.Common.Ultils;
using Application.Features.DaprService.Document;
using Application.Features.DaprService.Enrollment;
using Application.Features.DaprService.Flashcard;
using Application.Features.DaprService.Lesson;
using Application.Features.DaprService.Subject;
using Application.Features.DaprService.SubjectCurriculum;
using Application.Features.DaprService.Theory;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public class DaprEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/dapr");
        group.MapGet("document-ids", GetDocumentIdsDapr).WithName(nameof(GetDocumentIdsDapr));
        group.MapGet("document-tips", GetDocumentTipsDapr).WithName(nameof(GetDocumentTipsDapr));
        group.MapGet("flashcard-ids", GetFlashcardIdsDapr).WithName(nameof(GetFlashcardIdsDapr));
        group.MapGet("flashcard-tips", GetFlashcardTipsDapr).WithName(nameof(GetFlashcardTipsDapr));
        group.MapGet("check-resources", CheckResourceExistsDapr).WithName(nameof(CheckResourceExistsDapr));
        group.MapGet("enrollment", GetEnrollmentDapr).WithName(nameof(GetEnrollmentDapr));
        group.MapGet("user-flashcard-learning", GetUserFlashcardLearningDapr).WithName(nameof(GetUserFlashcardLearningDapr));
        group.MapGet("lesson-ids", GetLessonIdsDapr).WithName(nameof(GetLessonIdsDapr));
        group.MapGet("check-lesson-exit", CheckLessonExitDapr).WithName(nameof(CheckLessonExitDapr));
        group.MapGet("check-subject-curriculum-name", CheckSubjectCurriculumIdDapr).WithName(nameof(CheckSubjectCurriculumIdDapr));
		group.MapGet("check-subject-curriculum-id/subject/{subjectId}/curriculum/{curriculumId}", GetSubjectCurriculumIdDapr)
            .WithName(nameof(GetSubjectCurriculumIdDapr));
		group.MapGet("check-subject-name", CheckSubjectIdDapr).WithName(nameof(CheckSubjectIdDapr));
        group.MapGet("subject-grade", GetSubjectGradeDapr).WithName(nameof(GetSubjectGradeDapr));
        group.MapGet("theory-tips", GetTheoryTipsDapr).WithName(nameof(GetTheoryTipsDapr));
	}

	public static async Task<IResult> GetDocumentIdsDapr([FromQuery] string[] subjectIds,
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetDocumentId()
        {
            SubjectIds = subjectIds
        };
        var result = await sender.Send(query, cancellationToken);

		return JsonHelper.Json(result);
    }

    public static async Task<IResult> GetDocumentTipsDapr([FromQuery] string[] documentIds,
         ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetDocumentTips()
        {
            DocumentIds = documentIds
        };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetFlashcardIdsDapr([FromQuery] string[] subjectIds,
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetFlashcardId()
        {
            SubjectIds = subjectIds
        };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetFlashcardTipsDapr([FromQuery] string[] flashcardIds,
         ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetFlashcardTips()
        {
            FlaschcardId = flashcardIds
        };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetEnrollmentDapr(
         ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetEnrollment()
        {
        };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetUserFlashcardLearningDapr(
         ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetUserFlashcardLearning()
        {
        };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }

    public static async Task<IResult> CheckResourceExistsDapr([AsParameters]CheckResourceExistsRequestDapr request,
       ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprCheckResourceExists()
        {
            Check = request
        };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetLessonIdsDapr([FromQuery] string[] subjectIds,
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetLessonId()
        {
            SubjectIds = subjectIds
        };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CheckLessonExitDapr([FromQuery] string[] lessonIds,
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprCheckLessonExit()
        {
            LessonIds = lessonIds
        };
        var result = await sender.Send(query, cancellationToken);

        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CheckSubjectCurriculumIdDapr([FromQuery] string[] subjectCurriculumId,
         ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprCheckSubjectCurriculumId()
        {
            SubjectCurriculumId = subjectCurriculumId
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
	public static async Task<IResult> GetSubjectCurriculumIdDapr(Guid subjectId, Guid curriculumId,
		 ISender sender, CancellationToken cancellationToken)
	{
		var query = new CheckSubjectAndCurriculum()
		{
			CurriculumId = curriculumId,
            SubjectId = subjectId
		};
		var result = await sender.Send(query, cancellationToken);
		return JsonHelper.Json(result);
	}
	public static async Task<IResult> CheckSubjectIdDapr([FromQuery] string[] subjectId,
         ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprCheckSubjectId()
        {
            SubjectId = subjectId
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetSubjectGradeDapr([FromQuery] string[] subjectId,
         ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetSubjectGrade()
        {
            SubjectId = subjectId
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetTheoryTipsDapr([FromQuery] string[] theoryId,
         ISender sender, CancellationToken cancellationToken)
    {
        var query = new DaprGetTheoryTips()
        {
            TheoryIds = theoryId
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
}