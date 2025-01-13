using Application.Common.Ultils;
using Application.Features.RecommendedFeature.Queries;
using Carter;

namespace Application.Endpoints;

public class RecommendEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/data");
        group.MapGet("recommended",GetRecommendedData).RequireAuthorization("studentPolicy").WithName(nameof(GetRecommendedData));
        group.MapGet("top",GetTopData).WithName(nameof(GetTopData));
        
    }
    public static async Task<IResult> GetRecommendedData(ISender sender, CancellationToken cancellationToken)
    {
        var query = new RecommendedDataUserQueries() { };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    
    public static async Task<IResult> GetTopData(ISender sender, CancellationToken cancellationToken)
    {
        var query = new TopDataQuery() { };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
}