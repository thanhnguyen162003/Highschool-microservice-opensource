using Application.Common.Models.ChapterModel;
using Application.Common.Ultils;
using Application.Features.ChapterFeature.Commands.V2;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints.V2;

public class ChapterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v2");
        group.MapPost("chapter/subject/{subjectId}/curriculum/{curriculumId}", CreateChapterSingleV2).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateChapterSingleV2));
        group.MapPost("chapters/subject/{subjectId}/curriculum/{curriculumId}", CreateChapterListV2).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateChapterListV2));
    }
    public static async Task<IResult> CreateChapterSingleV2([FromBody] ChapterCreateRequestModel chapterCreateRequestModel, ISender sender,
        Guid subjectId, Guid curriculumId, ValidationHelper<ChapterCreateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(chapterCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateChapterCommand()
        {
            ChapterCreateRequestModel = chapterCreateRequestModel,
			SubjectId = subjectId,
			CurriculumId = curriculumId
		};
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    
    public static async Task<IResult> CreateChapterListV2([FromBody] List<ChapterCreateRequestModel> chapterCreateRequestModel, ISender sender,
		Guid subjectId, Guid curriculumId, ValidationHelper<List<ChapterCreateRequestModel>> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(chapterCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateChapterListCommand()
        {
            ListChapterCreateRequestModels = chapterCreateRequestModel,
			SubjectId = subjectId,
			CurriculumId = curriculumId
		};
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
}