using Application.Common.Models.SubjectModel;
using Application.Common.Ultils;
using Application.Features.SubjectCurriculumFeature.Commands;
using Application.Features.SubjectCurriculumFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class SubjectCurriculumEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("subject/{subjectId}/curriculum/{curriculumId}",GetSubjectCurriculumId).WithName(nameof(GetSubjectCurriculumId));
		group.MapGet("check/subject/{subjectId}/curriculum/{curriculumId}", CheckSubjectCurriculumId).WithName(nameof(CheckSubjectCurriculumId));
		group.MapGet("subject-curriculum/publish", GetSubjectsPublish).WithName(nameof(GetSubjectsPublish));
        group.MapGet("subject-curriculum/unpublish", GetSubjectsUnPublish).RequireAuthorization("moderatorPolicy").RequireAuthorization("moderatorPolicy").WithName(nameof(GetSubjectsUnPublish));
        group.MapPatch("subject-curriculum/{id}/publish",PublishSubject).RequireAuthorization("moderatorPolicy").WithName(nameof(PublishSubject));
        group.MapPatch("subject-curriculum/{id}/unpublish",UnPublishSubject).RequireAuthorization("moderatorPolicy").WithName(nameof(UnPublishSubject));
    }

    public static async Task<IResult> GetSubjectCurriculumId(Guid subjectId, Guid curriculumId,ISender sender, CancellationToken cancellationToken)
    {
        var query = new SubjectCurriculumIdQuery()
        {
            CurriculumId = curriculumId,
            SubjectId = subjectId
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
	public static async Task<IResult> CheckSubjectCurriculumId(Guid subjectId, Guid curriculumId, ISender sender, CancellationToken cancellationToken)
	{
		var query = new SubjectCurriculumCheckQuery()
		{
			CurriculumId = curriculumId,
			SubjectId = subjectId
		};
		var result = await sender.Send(query, cancellationToken);
		return JsonHelper.Json(result);
	}
	public static async Task<IResult> PublishSubject(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new PublishSubjectCurriculumCommand()
        {
            subjectCurriculumId = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> UnPublishSubject(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new UnPublishSubjectCurriculumCommand()
        {
            subjectCurriculumId = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> GetSubjectsUnPublish([AsParameters] SubjectCurriculumQueryFilter queryFilter, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new SubjectCurriculumQueryUnPublish()
        {
            QueryFilter = queryFilter
        };
        var result = await sender.Send(query, cancellationToken);
        var metadata = new Metadata
        {
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages
        };
        httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetSubjectsPublish([AsParameters] SubjectCurriculumQueryFilter queryFilter, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new SubjectCurriculumQueryPublish()
        {
            QueryFilter = queryFilter
        };
        var result = await sender.Send(query, cancellationToken);
        var metadata = new Metadata
        {
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages
        };
        httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        return JsonHelper.Json(result);
    }
}