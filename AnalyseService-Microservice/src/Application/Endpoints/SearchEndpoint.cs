using Application.Common.Models;
using Application.Common.Ultils;
using Application.Features.SearchFeature.Queries;
using Application.Services.Search;
using Carter;
using Domain.CustomModel;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class SearchEndpoints : ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/search");
        group.MapGet("", SearchFlashCard).WithName(nameof(SearchFlashCard));
        group.MapGet("courses", SearchCourse).WithName(nameof(SearchCourse));
    }

    private static async Task<IResult> SearchFlashCard([AsParameters]SearchQuery searchQuery, ISender sender, CancellationToken cancellationToken, HttpContext context)
    {
        var result = await sender.Send(searchQuery, cancellationToken);

        if (searchQuery.Type == SearchType.All)
        {
            return JsonHelper.Json(result);
        } else
        {
            context.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(new Metadata()
            {
                CurrentPage = searchQuery.PageNumber + 1,
                PageSize = searchQuery.PageSize,
                TotalPages = (int)result.GetType().GetProperty("TotalPages")?.GetValue(result)!,
                TotalCount = (int)result.GetType().GetProperty("TotalCount")?.GetValue(result)!
            }));

            return JsonHelper.Json(result);
        }

    }

    private static async Task<IResult> SearchCourse([AsParameters] SearchCourseQuery searchCourseQuery, ISender sender, CancellationToken cancellationToken, HttpContext context)
    {
        var result = await sender.Send(searchCourseQuery, cancellationToken);

        return JsonHelper.Json(result);
    }
}
