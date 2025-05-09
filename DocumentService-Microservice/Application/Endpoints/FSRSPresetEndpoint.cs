using Application.Common.Models.ChapterModel;
using Application.Common.Models.FSRSPresetModel;
using Application.Common.Ultils;
using Application.Features.ChapterFeature.Commands;
using Application.Features.ChapterFeature.Queries;
using Application.Features.FSRSPresetFeature.Commands;
using Application.Features.FSRSPresetFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class FSRSPresetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("preset",GetPreset).RequireAuthorization().WithName(nameof(GetPreset));
        group.MapPost("preset",CreatePreset).RequireAuthorization().WithName(nameof(CreatePreset));
        group.MapPut("preset/{id}", UpdatePreset).RequireAuthorization().WithName(nameof(UpdatePreset));
        group.MapDelete("preset/{id}",DeletePreset).RequireAuthorization().WithName(nameof(DeletePreset));

    }
    public static async Task<IResult> GetPreset([AsParameters] FSRSPresetQueryFilter queryFilter, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new FSRSPresetQuery()
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
    
    public static async Task<IResult> CreatePreset([FromBody] FSRSPresetCreateRequest request, ISender sender, ValidationHelper<FSRSPresetCreateRequest> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(request);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new FSRSPresetCreateCommand()
        {
            fSRSPresetCreate = request
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> UpdatePreset([FromBody] FSRSPresetCreateRequest request, ISender sender,
        Guid id, ValidationHelper<FSRSPresetCreateRequest> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(request);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new FSRSPresetUpdateCommand()
        {
            Id = id,
            fSRSPresetCreate = request
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Status);
    }
    public static async Task<IResult> DeletePreset(ISender sender,
       Guid id, CancellationToken cancellationToken)
    {
        var command = new FSRSPresetDeleteCommand()
        {
            Id = id
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Status);
    }
}