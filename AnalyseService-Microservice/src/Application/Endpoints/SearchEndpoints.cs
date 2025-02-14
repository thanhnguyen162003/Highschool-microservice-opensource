using Application.Common.Ultils;
using Application.Features.SearchFeature.Queries;
using Application.Services.Search;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public class SearchEndpoints : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("search", SearchFlashCard).WithName(nameof(SearchFlashCard));
    }

    private static async Task<IResult> SearchFlashCard([AsParameters]SearchQuery searchQuery, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(searchQuery, cancellationToken);

        return JsonHelper.Json(result);
    }
}
