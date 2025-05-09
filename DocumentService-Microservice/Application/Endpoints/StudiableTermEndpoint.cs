using Application.Common.Models.StudiableTermModel;
using Application.Common.Ultils;
using Application.Features.StudiableTermFeature.Commands;
using Application.Features.StudiableTermFeature.Queries;
using Carter;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Application.Endpoints;

public class StudiableTermEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/studiableTerm");
        group.MapPost("list",CreateStudiableTermList).RequireAuthorization().WithName(nameof(CreateStudiableTermList));
        group.MapPut("list", UpdateStudiableTermList).RequireAuthorization().WithName(nameof(UpdateStudiableTermList));
        group.MapPost("", CreateStudiableTerm).RequireAuthorization().WithName(nameof(CreateStudiableTerm));
        group.MapPut("", UpdateStudiableTerm).RequireAuthorization().WithName(nameof(UpdateStudiableTerm));
        group.MapDelete("{flashcardId}", ResetFlashcard).RequireAuthorization().WithName(nameof(ResetFlashcard));
        group.MapGet("{flashcardId}", GetFlashcardSort).RequireAuthorization().WithName(nameof(GetFlashcardSort));

    }
    public static async Task<IResult> CreateStudiableTermList([Required]List<StudiableTermRequestModel> studiableTermRequestModels, ISender sender, CancellationToken cancellationToken)
    {
        var command = new CreateStudiableTermListCommand()
        {
            StudiableTermRequestModels = studiableTermRequestModels
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> CreateStudiableTerm([Required] StudiableTermRequestModel studiableTermRequestModel, ISender sender, CancellationToken cancellationToken)
    {
        var command = new CreateStudiableTermCommand()
        {
            StudiableTermRequestModels = studiableTermRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> ResetFlashcard(Guid flashcardId, ISender sender, CancellationToken cancellationToken)
    {
        var command = new ResetFlashcardCommand()
        {
            FlashcardId = flashcardId
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> UpdateStudiableTerm([Required] StudiableTermRequestModel studiableTermRequestModel, ISender sender, CancellationToken cancellationToken)
    {
        var command = new UpdateStudiableTermCommand()
        {
            StudiableTermRequestModels = studiableTermRequestModel
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    public static async Task<IResult> UpdateStudiableTermList([Required] List<StudiableTermRequestModel> studiableTermRequestModels, ISender sender, CancellationToken cancellationToken)
    {
        var command = new UpdateStudiableTermListCommand()
        {
            StudiableTermRequestModels = studiableTermRequestModels
        };
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }


    public static async Task<IResult> GetFlashcardSort(Guid flashcardId, ISender sender,
        IMapper mapper, CancellationToken cancellationToken)
    {
        var query = new FlashcardSortQuery()
        {
            FlashcardId = flashcardId
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }
}