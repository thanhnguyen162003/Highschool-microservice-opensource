using System.ComponentModel.DataAnnotations;
using Application.Common.Ultils;
using Application.Features.BaseFeature.Queries;
using Carter;

namespace Application.Endpoints;

public class DocumentRoadmapEndpoints : ICarterModule
{
    //this just for demo purpose, for production need somethings faster
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/roadmap/node/data");
        group.MapGet("{id}",GetDataInNodeRoadmap).WithName(nameof(GetDataInNodeRoadmap));
    }
    public static async Task<IResult> GetDataInNodeRoadmap([Required] Guid id,
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new DataQuery()
        {
            Id = id
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
}