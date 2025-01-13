using Application.Common.Models;
using Application.Common.Models.RoadmapDataModel;
using Application.Common.Ultils;
using Application.Features.RoadmapFeature.Commands;
using Application.Features.RoadmapFeature.Queries;
using Carter;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class RoadmapEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapPost("roadmap/detail",CreateRoadmapDetail).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateRoadmapDetail));
        group.MapPost("roadmap",CreateRoadmap).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateRoadmap));
        group.MapGet("roadmaps",GetRoadmapFilter).WithName(nameof(GetRoadmapFilter));
        group.MapGet("roadmap/{id}",GetRoadmapDetailById).WithName(nameof(GetRoadmapDetailById));
    }

    public static async Task<IResult> CreateRoadmapDetail([FromBody] RoadMapSectionCreateRequestModel roadMapSectionCreateRequestModel, ISender sender,
        CancellationToken cancellationToken, ValidationHelper<RoadMapSectionCreateRequestModel> validator)
    {
        var (isValid, response) = await validator.ValidateAsync(roadMapSectionCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new RoadmapDetailCreateCommand()
        {
            RoadMapSectionCreateCommand = roadMapSectionCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CreateRoadmap([FromBody] RoadmapCreateRequestModel roadmapCreateRequestModel, ISender sender,
        CancellationToken cancellationToken, ValidationHelper<RoadmapCreateRequestModel> validator)
    {
        var (isValid, response) = await validator.ValidateAsync(roadmapCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateRoadmapCommand()
        {
            RoadmapCreateRequestModel = roadmapCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetRoadmapFilter([AsParameters] RoadmapQueryFilter roadmapQueryFilter, ISender sender,
        CancellationToken cancellationToken, HttpContext httpContext, IMapper mapper)
    {
        var query = new RoadmapQuery()
        {
            QueryFilter = roadmapQueryFilter
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
    public static async Task<IResult> GetRoadmapDetailById(string id, ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new RoadmapDetailQuery()
        {
            RoadmapId = id
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
}
