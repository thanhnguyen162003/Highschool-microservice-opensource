using Application.Common.Models.CurriculumModel;
using Application.Common.Ultils;
using Application.Features.CurrilumFeature.Commands;
using Application.Features.CurrilumFeature.Queries;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public class CurriculumEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("curriculum",GetCurriculum).WithName(nameof(GetCurriculum));
        group.MapPost("curriculum",CreateCurriculum).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateCurriculum));
    }

    public static async Task<IResult> GetCurriculum(ISender sender, CancellationToken cancellationToken)
    {
        var query = new CurrilumQuery()
        {
            
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CreateCurriculum([FromBody] CurriculumCreateRequestModel curriculumCreateRequestModel,
        ISender sender, CancellationToken cancellationToken, ValidationHelper<CurriculumCreateRequestModel> validationHelper)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(curriculumCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateCurrilumCommand()
        {
            CurriculumCreateRequestModel = curriculumCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
}