using Application.Common.Models.CurriculumModel;
using Application.Common.Ultils;
using Application.Features.CurrilumFeature.Commands;
using Application.Features.CurrilumFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class CurriculumEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("curriculum",GetCurriculum).WithName(nameof(GetCurriculum));
        group.MapPost("curriculum",CreateCurriculum).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateCurriculum));
		group.MapDelete("curriculum", DeleteCurriculum).RequireAuthorization("moderatorPolicy").WithName(nameof(DeleteCurriculum));
	}

	public static async Task<IResult> GetCurriculum([AsParameters] CurriculumQueryFilter queryFilter,
        ISender sender, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new CurrilumQuery()
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
	public static async Task<IResult> DeleteCurriculum(ISender sender, CancellationToken cancellationToken, Guid id)
	{
		var command = new CurriculumDeleteCommand()
		{
			Id = id
		};
		var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status!);
	}
}