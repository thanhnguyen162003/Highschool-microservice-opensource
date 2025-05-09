using Application.Common.Ultils;
using Application.Features.AnalyseFeature.Queries;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public class AnalyzeEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        // group.MapPost("roadmap/detail", CreateRoadmapDetail).RequireAuthorization("moderatorPolicy")
        //     .WithName(nameof(CreateRoadmapDetail));
        group.MapGet("analytics/flashcard", GetFlashcardAnalytic)
            .WithName(nameof(GetFlashcardAnalytic));

    }
    //
    // public static async Task<IResult> CreateRoadmapDetail([FromBody] RoadMapSectionCreateRequestModel roadMapSectionCreateRequestModel, ISender sender,
    //     CancellationToken cancellationToken, ValidationHelper<RoadMapSectionCreateRequestModel> validator)
    // {
    //     var (isValid, response) = await validator.ValidateAsync(roadMapSectionCreateRequestModel);
    //     if (!isValid)
    //     {
    //         return Results.BadRequest(response);
    //     }
    //     var command = new RoadmapDetailCreateCommand()
    //     {
    //         RoadMapSectionCreateCommand = roadMapSectionCreateRequestModel
    //     };
    //     var result = await sender.Send(command, cancellationToken);
    //     return JsonHelper.Json(result);
    // }
    public static async Task<IResult> GetFlashcardAnalytic(
        [FromQuery] Guid userId,
        [FromQuery] Guid? flashcardId = null,
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? maxResults = null,
        [FromQuery] string sortBy = null,
        [FromQuery] bool includeSessionDetails = true,
        ISender sender = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserFlashcardAnalyticsQuery()
        {
            UserId = userId,
            FlashcardId = flashcardId,
            StartDate = startDate,
            EndDate = endDate,
            MaxResults = maxResults,
            SortBy = sortBy,
            IncludeSessionDetails = includeSessionDetails
        };
    
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    
}
