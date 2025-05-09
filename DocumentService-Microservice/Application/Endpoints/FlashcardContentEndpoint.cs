using System.ComponentModel.DataAnnotations;
using System.Net;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.Ultils;
using Application.Features.FlashcardContentFeature.Commands;
using Application.Features.FlashcardContentFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class FlashcardContentEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/flashcard");
        group.MapGet("{id}/contents",GetFlashcardContents).WithName(nameof(GetFlashcardContents));
        group.MapGet("/slug/{slug}/contents",GetFlashcardContentsSlug).WithName(nameof(GetFlashcardContentsSlug));
        group.MapPost("{id}/contents",CreateFlashcardContents).RequireAuthorization().WithName(nameof(CreateFlashcardContents));
        group.MapPost("{id}/content",CreateFlashcardContent).RequireAuthorization().WithName(nameof(CreateFlashcardContent));
        group.MapPatch("{id}/contents",UpdateListFlashcardContent).RequireAuthorization().WithName(nameof(UpdateListFlashcardContent));
        group.MapPatch("{id}/content",UpdateSingleFlashcardContent).RequireAuthorization().WithName(nameof(UpdateSingleFlashcardContent));
        group.MapDelete("{id}/contents/{flashcardId}",DeleteFlashcardContent).RequireAuthorization().WithName(nameof(DeleteFlashcardContent));
        group.MapPatch("content/{id}/rank/{rank}",SwapFlashcardContent).RequireAuthorization().WithName(nameof(SwapFlashcardContent));
    }
    public static async Task<IResult> GetFlashcardContents([AsParameters] FlashcardQueryFilter queryFilter, [Required] Guid id,
        ISender sender, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new FlashcardContentQuery()
        {
            QueryFilter = queryFilter,
            FlashcardId = id
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
    
    public static async Task<IResult> GetFlashcardContentsSlug([AsParameters] FlashcardQueryFilter queryFilter, [FromRoute] string slug,
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetFlashcardContentSlugQuery()
        {
            QueryFilter = queryFilter,
            Slug = slug
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> CreateFlashcardContents(Guid id, [FromBody] List<FlashcardContentListCreateRequestModel> listFlashcardContent, ISender sender,
        ValidationHelper<List<FlashcardContentCreateRequestModel>> validationHelper, CancellationToken cancellationToken)
    {
        //var (isValid, response) = await validationHelper.ValidateAsync(listFlashcardContent);
        //if (!isValid)
        //{
        //    return Results.BadRequest(response);
        //}
        var query = new CreateFlashcardContentCommand()
        {
            FlashcardId = id,
            FlashcardContentCreateRequestModel = listFlashcardContent
        };
        var result = await sender.Send(query);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> CreateFlashcardContent(Guid id, [FromBody] FlashcardContentCreateRequestModel flashcardContentCreate, ISender sender,
        ValidationHelper<FlashcardContentCreateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(flashcardContentCreate);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var query = new CreateFlashcardContentSingleCommand()
        {
            FlashcardId = id,
            FlashcardContentCreateRequestModel = flashcardContentCreate
        };
        var result = await sender.Send(query);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    
    public static async Task<IResult> UpdateListFlashcardContent(Guid id, [FromBody] List<FlashcardContentUpdateRequestModel> listFlashcardContentUpdate, ISender sender,
        ValidationHelper<List<FlashcardContentUpdateRequestModel>> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(listFlashcardContentUpdate);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var query = new UpdateFlashcardContentCommand()
        {
            FlashcardId = id,
            FlashcardContentUpdateRequestModel = listFlashcardContentUpdate
        };
        var result = await sender.Send(query, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> UpdateSingleFlashcardContent(Guid id, [FromBody] FlashcardContentUpdateRequestModel singleFlashcardContentUpdate, ISender sender,
        ValidationHelper<FlashcardContentUpdateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(singleFlashcardContentUpdate);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var query = new UpdateFlashcardContentSingleCommand()
        {
            FlashcardId = id,
            FlashcardUpdateRequestModel = singleFlashcardContentUpdate
        };
        var result = await sender.Send(query, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> DeleteFlashcardContent([FromRoute] Guid id,[FromRoute] Guid flashcardId,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new DeleteFlashcardContentCommand()
        {
            FlashcardContentId = flashcardId,
            FlashcardId = id
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    public static async Task<IResult> SwapFlashcardContent([FromRoute] Guid id,[FromRoute] int rank,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new ReorderFlashcardContentCommand()
        {
            FlashcardContentId = id,
            NewRank = rank
        };
        var result = await sender.Send(command, cancellationToken);
		return Results.Json(result, statusCode: (int)result.Status);
	}
    
}