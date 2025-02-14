using Application.Common.Models.TheoryModel;
using Application.Common.Ultils;
using Application.Features.TheoryFeature.Commands;
using Application.Features.TheoryFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class TheoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/theory");
        group.MapDelete("{id}",DeleteTheory).RequireAuthorization("moderatorPolicy").WithName(nameof(DeleteTheory));
        group.MapPost("/lesson/{id}",CreateTheory).RequireAuthorization("moderatorPolicy").WithName(nameof(CreateTheory));
        group.MapPatch("{id}",UpdateTheory).RequireAuthorization("moderatorPolicy").WithName(nameof(UpdateTheory));
        group.MapGet("lesson/{id}",GetTheory).WithName(nameof(GetTheory));
        group.MapGet("{id}",GetTheoryById).WithName(nameof(GetTheoryById));
        
    }
    public static async Task<IResult> DeleteTheory(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new TheoryDeleteCommand()
        {
            TheoryId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> CreateTheory([FromBody] TheoryCreateRequestModel theoryCreateRequestModel, ISender sender,
        IMapper mapper, ValidationHelper<TheoryCreateRequestModel> validationHelper, CancellationToken cancellationToken, Guid id)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(theoryCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new TheoryCreateCommand()
        { 
           TheoryCreateRequestModel = theoryCreateRequestModel,
           LessonId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> UpdateTheory([FromBody] TheoryUpdateRequestModel theoryUpdateRequestModel, ISender sender,
        IMapper mapper, ValidationHelper<TheoryUpdateRequestModel> validationHelper, CancellationToken cancellationToken, Guid id)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(theoryUpdateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new TheoryUpdateCommand()
        { 
            TheoryUpdateRequestModel = theoryUpdateRequestModel,
            TheoryId = id
            
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> GetTheory([AsParameters] TheoryQueryFilter queryFilter, ISender sender,
        CancellationToken cancellationToken, HttpContext httpContext, Guid id)
    {
        var query = new TheoryQuery()
        {
            QueryFilter = queryFilter,
            LessonId = id
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
    public static async Task<IResult> GetTheoryById(Guid id, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new TheoryDetailQuery()
        {
            TheoryId = id
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
}