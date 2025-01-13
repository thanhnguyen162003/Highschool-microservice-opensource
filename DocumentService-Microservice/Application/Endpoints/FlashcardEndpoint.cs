using System.ComponentModel.DataAnnotations;
using Application.Common.Models.FlashcardModel;
using Application.Common.Ultils;
using Application.Features.FlashcardFeature.Commands;
using Application.Features.FlashcardFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.QueriesFilter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Application.Endpoints;

public class FlashcardEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/flashcard");
        group.MapGet("",GetFlashcards).WithName(nameof(GetFlashcards));
        group.MapGet("/top",GetTopFlashcards).WithName(nameof(GetTopFlashcards));
        //TODO:consider get related by flashcardid
		group.MapGet("/related", UserAlsoWatch).WithName(nameof(UserAlsoWatch));
		group.MapPost("",CreateFlashcard).RequireAuthorization().WithName(nameof(CreateFlashcard));
        group.MapGet("user/{username}",GetFlashcardByUsername).WithName(nameof(GetFlashcardByUsername));
        group.MapGet("user",GetFlashcardsOwn).RequireAuthorization().WithName(nameof(GetFlashcardsOwn));
        group.MapPost("draft",CreateFlashcardDraft).RequireAuthorization().WithName(nameof(CreateFlashcardDraft));
        group.MapGet("slug/{slug}",GetFlashcardBySlug).WithName(nameof(GetFlashcardBySlug));
        group.MapGet("{id}",GetFlashcardById).WithName(nameof(GetFlashcardById));
        group.MapGet("/draft/{id}",GetFlashcardDraftById).RequireAuthorization().WithName(nameof(GetFlashcardDraftById));
        group.MapPatch("{id}",UpdateFlashcard).RequireAuthorization().WithName(nameof(UpdateFlashcard));
        group.MapPatch("rating/{id}", VoteFlashcard).RequireAuthorization().WithName(nameof(VoteFlashcard));
        group.MapPatch("created/{id}",UpdateFlashcardCreated).RequireAuthorization().WithName(nameof(UpdateFlashcardCreated));
        group.MapDelete("{id}",DeleteFlashcard).RequireAuthorization().WithName(nameof(DeleteFlashcard));
    }

    public static async Task<IResult> GetFlashcards([AsParameters] FlashcardQueryFilter queryFilter, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new FlashcardQuery()
        {
            QueryFilter = queryFilter
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
        var mapResult = mapper.Map<IEnumerable<FlashcardRecommendResponseModel>>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> GetFlashcardByUsername([AsParameters] FlashcardQueryFilter queryFilter, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext,[Required] string username)
    {
        var query = new FlashcardQueryUser()
        {
            QueryFilter = queryFilter,
            Username = username
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
        var mapResult = mapper.Map<IEnumerable<FlashcardRecommendResponseModel>>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> GetFlashcardsOwn([AsParameters] FlashcardQueryFilter queryFilter, ISender sender,
        IMapper mapper, CancellationToken cancellationToken, HttpContext httpContext)
    {
        var query = new FlashcardOwnQuery()
        {
            QueryFilter = queryFilter
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
        var mapResult = mapper.Map<IEnumerable<FlashcardRecommendResponseModel>>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> CreateFlashcard([FromBody] FlashcardCreateRequestModel flashcardCreateRequestModel, ISender sender,
        ValidationHelper<FlashcardCreateRequestModel> validationHelper, CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(flashcardCreateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new CreateFlashcardCommand()
        {
            FlashcardCreateRequestModel = flashcardCreateRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> CreateFlashcardDraft(ISender sender, CancellationToken cancellationToken)
    {
        var command = new FlashcardDraftCreateCommand();
        var result = await sender.Send(command,cancellationToken);
        return JsonHelper.Json(result);
    }
    public static async Task<IResult> GetFlashcardBySlug([Required]string slug, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new FlashcardDetailSlugQuery()
        {
            Slug = slug
        };
        var result = await sender.Send(query, cancellationToken);
        var mapResult = mapper.Map<FlashcardResponseModel>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> GetFlashcardById([Required]Guid id, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new FlashcardDetailQuery()
        {
            flashcardId = id
        };
        var result = await sender.Send(query, cancellationToken);
        var mapResult = mapper.Map<FlashcardResponseModel>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> GetFlashcardDraftById([Required]Guid id, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new FlashcardDraftQuery()
        {
            FlashcardId = id
        };
        var result = await sender.Send(query, cancellationToken);
        var mapResult = mapper.Map<FlashcardResponseModel>(result);
        return JsonHelper.Json(mapResult);
    }
    public static async Task<IResult> UpdateFlashcard([FromBody] FlashcardUpdateRequestModel flashcardUpdateRequestModel,
        [Required] Guid id, ISender sender, IMapper mapper, ValidationHelper<FlashcardUpdateRequestModel> validationHelper,
        CancellationToken cancellationToken)
    {
        var (isValid, response) = await validationHelper.ValidateAsync(flashcardUpdateRequestModel);
        if (!isValid)
        {
            return Results.BadRequest(response);
        }
        var command = new UpdateFlashcardCommand()
        {
            FlashcardUpdateRequestModel = flashcardUpdateRequestModel,
            FlashcardId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> UpdateFlashcardCreated([Required] Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new UpdateCreatedFlashcardCommand()
        {
            FlashcardId = id
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> VoteFlashcard([Required] Guid flashcardId, [AsParameters] FlashcardVoteModel rating,ISender sender, IMapper mapper, CancellationToken cancellationToken)
    {
        var command = new LikeFlashcardCommand()
        {
            FlashcardId = flashcardId,
            FlashcardVoteModel = rating,
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> GetTopFlashcards(ISender sender, IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new FlashcardTopQuery() { };
        var result = await sender.Send(query, cancellationToken);
        var mapResult = mapper.Map<IEnumerable<FlashcardRecommendResponseModel>>(result);
        return JsonHelper.Json(mapResult);
    }
	public static async Task<IResult> UserAlsoWatch(ISender sender, IMapper mapper, CancellationToken cancellationToken)
	{
		var query = new FlashcardQueryAlsoWatch() { };
		var result = await sender.Send(query, cancellationToken);
		var mapResult = mapper.Map<IEnumerable<FlashcardRecommendResponseModel>>(result);
		return JsonHelper.Json(mapResult);
	}
	public static async Task<IResult> DeleteFlashcard(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new DeleteFlashcardCommand()
        {
            FlashcardId = id
        };
        var result = await sender.Send(command,cancellationToken);
        return JsonHelper.Json(result);
    }

}