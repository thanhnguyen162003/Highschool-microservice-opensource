using Application.Common.Models.LessonModel;
using Application.Common.Ultils;
using Application.Features.LessonFeature.Commands;
using Application.Features.LessonFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class LessonEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("chapter/{id}/lessons",GetLessonsByChapter).WithName(nameof(GetLessonsByChapter));
        group.MapGet("chapter/lesson/{id}",GetLessonsDetails).WithName(nameof(GetLessonsDetails));
        group.MapPost("chapter/{id}/lesson",CreateLessonSingle).WithName(nameof(CreateLessonSingle));
        group.MapPost("chapter/{id}/lessons",CreateLessonList).WithName(nameof(CreateLessonList));
        group.MapDelete("lessons",DeleteLesson).WithName(nameof(DeleteLesson));
        group.MapPatch("lesson",UpdateLesson).WithName(nameof(UpdateLesson));

    }

    public static async Task<IResult> GetLessonsByChapter([AsParameters] LessonQueryFilter queryFilter,Guid id, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new LessonQuery()
        {
            QueryFilter = queryFilter,
            ChapterId = id
        };
        var result = await sender.Send(query,cancellationToken);
        var metadata = new Metadata
        {
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage,
            TotalPages = result.TotalPages
        };
        httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var mappedData = mapper.Map<IEnumerable<LessonResponseModel>>(result);
        return JsonHelper.Json(mappedData);
    }
    public static async Task<IResult> GetLessonsDetails(Guid id, ISender sender,
        IMapper mapper,CancellationToken cancellationToken)
    {
        var query = new LessonDetailQuery()
        {
            LessonId = id
        };
        var result = await sender.Send(query, cancellationToken);
        // var mappedData = mapper.Map<LessonDetailResponseModel>(result);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CreateLessonSingle([FromBody] LessonCreateRequestModel lessonCreateRequestModel,
        ISender sender, IMapper mapper, ValidationHelper<LessonCreateRequestModel> validationHelper,
        Guid id, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(lessonCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new LessonCreateSingleCommand()
        {
            LessonModel = mapper.Map<LessonModel>(lessonCreateRequestModel),
            ChapterId = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> CreateLessonList([FromBody] List<LessonCreateRequestModel> lessonCreateRequestModel,
        ISender sender, ValidationHelper<List<LessonCreateRequestModel>> validationHelper,
        Guid id, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(lessonCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new LessonCreateListCommand()
        {
            LessonCreateRequestModels = lessonCreateRequestModel,
            ChapterId = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> DeleteLesson([FromBody]List<Guid> id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new LessonDeleteCommand()
        {
            LessonId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> UpdateLesson([FromBody] LessonUpdateRequestModel lessonUpdateRequestModel, ISender sender,
        IMapper mapper, ValidationHelper<LessonUpdateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var mapSubject = mapper.Map<LessonUpdateRequestModel>(lessonUpdateRequestModel);
        var (isValid, response) = await validationHelper.ValidateAsync(mapSubject);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new LessonUpdateCommand()
        {
            LessonModel = mapper.Map<LessonModel>(lessonUpdateRequestModel),
            LessonId = lessonUpdateRequestModel.LessonId
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
}