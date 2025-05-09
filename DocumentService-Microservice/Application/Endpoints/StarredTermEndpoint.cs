using Application.Features.StarredTermFeature.Commands;
using Carter;

namespace Application.Endpoints;

public class StarredTermEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/starredTerm");
        group.MapPost("{id}",Star).RequireAuthorization().WithName(nameof(Star));
        //group.MapGet("lesson/{id}",GetTheory).WithName(nameof(GetTheory));
        //group.MapGet("{id}",GetTheoryById).WithName(nameof(GetTheoryById));
        
    }
    public static async Task<IResult> Star(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new CreateStarredTermCommand()
        {
            FlashcardContentId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    
    //public static async Task<IResult> GetTheory([AsParameters] TheoryQueryFilter queryFilter, ISender sender,
    //    CancellationToken cancellationToken, HttpContext httpContext, Guid id)
    //{
    //    var query = new TheoryQuery()
    //    {
    //        QueryFilter = queryFilter,
    //        LessonId = id
    //    };
    //    var result = await sender.Send(query, cancellationToken);
    //    var metadata = new Metadata
    //    {
    //        TotalCount = result.TotalCount,
    //        PageSize = result.PageSize,
    //        CurrentPage = result.CurrentPage,
    //        TotalPages = result.TotalPages
    //    };
    //    httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
    //    return JsonHelper.Json(result);
    //}
    //public static async Task<IResult> GetTheoryById(Guid id, ISender sender,
    //    IMapper mapper, CancellationToken cancellationToken)
    //{
    //    var query = new TheoryDetailQuery()
    //    {
    //        TheoryId = id
    //    };
    //    var result = await sender.Send(query, cancellationToken);
    //    return JsonHelper.Json(result);
    //}
}