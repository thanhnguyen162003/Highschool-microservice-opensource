using Carter;

namespace Application.Endpoints;

public class AnalyzeEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        // group.MapPost("roadmap/detail", CreateRoadmapDetail).RequireAuthorization("moderatorPolicy")
        //     .WithName(nameof(CreateRoadmapDetail));

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
    
}
