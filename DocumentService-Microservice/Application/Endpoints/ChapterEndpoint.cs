using Application.Common.Models.ChapterModel;
using Application.Common.Ultils;
using Application.Features.ChapterFeature.Commands;
using Application.Features.ChapterFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class ChapterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("chapter",GetChapters).WithName(nameof(GetChapters));
        group.MapGet("chapter/subject-curriculum/{id}",GetChaptersBySubjectCurriculumId).WithName(nameof(GetChaptersBySubjectCurriculumId));
        group.MapGet("chapter/subject/{subjectId}/curriculum/{curriculumId}",GetChaptersBySubject).WithName(nameof(GetChaptersBySubject));
        group.MapGet("chapter/subject/slug/{subjectSlug}/curriculum/{id}",GetChaptersBySubjectSlug).WithName(nameof(GetChaptersBySubjectSlug));
        group.MapPost("chapter/subject-curriculum/{id}",CreateChapterSingle).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateChapterSingle));
        group.MapPost("chapters/subject-curriculum/{id}",CreateChapterList).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateChapterList));
        group.MapPatch("chapter",UpdateChapterSingle).RequireAuthorization("moderatorPolicy").WithName(nameof(UpdateChapterSingle));
		group.MapDelete("chapter/{id}", DeleteChapter).RequireAuthorization("moderatorPolicy").WithName(nameof(DeleteChapter));
	}

    public static async Task<IResult> GetChapters([AsParameters] ChapterQueryFilter queryFilter, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new ChapterQuery()
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
    
    public static async Task<IResult> GetChaptersBySubject([AsParameters] ChapterQueryFilter queryFilter,Guid curriculumId, Guid subjectId, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new GetChapterBySubjectCurrilumQuery()
        {
            QueryFilter = queryFilter,
            SubjectId = subjectId,
            CurriculumId = curriculumId
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
    public static async Task<IResult> GetChaptersBySubjectCurriculumId([AsParameters] ChapterQueryFilter queryFilter,Guid id, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new GetChapterBySubjectCurriculumIdQuery()
        {
            QueryFilter = queryFilter,
            SubjectCurriculumId = id
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
    public static async Task<IResult> GetChaptersBySubjectSlug([AsParameters] ChapterQueryFilter queryFilter,string subjectSlug, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext, Guid id)
    {
        var query = new GetChapterBySubjectSlug()
        {
            QueryFilter = queryFilter,
            SubjectSlug = subjectSlug,
            CurriculumId = id
        };
        var result = await sender.Send(query, cancellationToken);
        var metadata = new Metadata
        {
            TotalCount = result.Items.TotalCount,
            PageSize = result.Items.PageSize,
            CurrentPage = result.Items.CurrentPage,
            TotalPages = result.Items.TotalPages
        };
        httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        return JsonHelper.Json(result);
    }
    
    public static async Task<IResult> CreateChapterSingle([FromBody] ChapterCreateRequestModel chapterCreateRequestModel, ISender sender,
        Guid id, ValidationHelper<ChapterCreateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(chapterCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateChapterCommand()
        {
            ChapterCreateRequestModel = chapterCreateRequestModel,
            SubjectCurriculumId = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    
    public static async Task<IResult> CreateChapterList([FromBody] List<ChapterCreateRequestModel> chapterCreateRequestModel, ISender sender,
        Guid id, ValidationHelper<List<ChapterCreateRequestModel>> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(chapterCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateChapterListCommand()
        {
            ListChapterCreateRequestModels = chapterCreateRequestModel,
            SubjectCurriculumId = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> UpdateChapterSingle([FromBody] ChapterUpdateRequestModel chapterUpdateRequestModel, ISender sender,
        IMapper mapper, ValidationHelper<ChapterUpdateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(chapterUpdateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new UpdateChapterCommand()
        {
            ChapterUpdateRequestModel = chapterUpdateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
	public static async Task<IResult> DeleteChapter(Guid id, ISender sender, CancellationToken cancellationToken)
	{
		var command = new DeleteChapterCommand()
		{
			Id = id
		};
		var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
}