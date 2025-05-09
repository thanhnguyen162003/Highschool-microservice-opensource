using Application.Common.Ultils;
using Application.Features.FlashcardFolderUserFeature.Commands;
using Application.Features.FlashcardFolderUserFeature.Queries;
using Carter;
using Domain.CustomEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Application.Endpoints
{
    public class FolderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/folders");
            group.MapGet("", GetFolderUser).WithName(nameof(GetFolderUser));
            group.MapGet("flashcards", GetFlashcardOnFolder).WithName(nameof(GetFlashcardOnFolder));
            group.MapPost("", CreateFlashcardFolderUser).RequireAuthorization().WithName(nameof(CreateFlashcardFolderUser));
            group.MapDelete("{folderId}", DeleteFolderUser).RequireAuthorization().WithName(nameof(DeleteFolderUser));
            group.MapDelete("flashcards/{flashcardId}", DeleteFlashcardFolder).RequireAuthorization().WithName(nameof(DeleteFlashcardFolder));
            group.MapPatch("{folderId}", UpdateFlashcardFolder).RequireAuthorization().WithName(nameof(UpdateFlashcardFolder));
            group.MapPost("{folderId}/add", AddToFolder).RequireAuthorization().WithName(nameof(AddToFolder));
            group.MapDelete("documents/{documentId}", DeleteDocumentFolder).RequireAuthorization().WithName(nameof(DeleteDocumentFolder));
        }
    
        private static async Task<IResult> GetFlashcardOnFolder([AsParameters] FlashcardFolderQuery flashcardFolderQuery, ISender sender,
            HttpContext httpContext, CancellationToken cancellationToken)
        {
            var result = await sender.Send(flashcardFolderQuery, cancellationToken);

            if (result.Status != HttpStatusCode.OK)
            {
                return Results.BadRequest(result);
            }

            return JsonHelper.Json(result);
        }

        private static async Task<IResult> GetFolderUser([AsParameters] FolderUserQuery folderUserQuery, ISender sender,
            HttpContext httpContext, CancellationToken cancellationToken)
        {
            var result = await sender.Send(folderUserQuery, cancellationToken);

            httpContext.Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(new Metadata()
            {
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            }));

            return JsonHelper.Json(result);
        }

        private static async Task<IResult> CreateFlashcardFolderUser([FromBody] CreateFolderCommand createFlashcardFolderCommand,
            ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(createFlashcardFolderCommand, cancellationToken);

            if(result.Status != HttpStatusCode.Created)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteFolderUser(Guid folderId,
            ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteFolderCommand()
            {
                FolderId = folderId
            }, cancellationToken);

            if (result.Status != HttpStatusCode.OK)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);

        }

        private static async Task<IResult> DeleteFlashcardFolder(Guid flashcardId, Guid folderId,
            ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteFlashcardFolderCommand()
            {
                FlashcardId = flashcardId,
                FolderId = folderId
            }, cancellationToken);

            if (result.Status != HttpStatusCode.OK)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteDocumentFolder(Guid documentId, Guid folderId,
                       ISender sender, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteDocumentFolderCommand()
            {
                DocumentId = documentId,
                FolderId = folderId
            }, cancellationToken);

            if (result.Status != HttpStatusCode.OK)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        }

        private static async Task<IResult> UpdateFlashcardFolder(Guid folderId, UpdateFlashcardFolderCommand updateFlashcardFolderCommand,
            ISender sender, CancellationToken cancellationToken)
        {
            updateFlashcardFolderCommand.FolderId = folderId;
            var result = await sender.Send(updateFlashcardFolderCommand, cancellationToken);

            if (result.Status != HttpStatusCode.OK)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        }

        private static async Task<IResult> AddToFolder(Guid folderId, AddToFolderCommand addToFolderCommand, 
            ISender sender, CancellationToken cancellationToken)
        {
            addToFolderCommand.FolderId = folderId;
            var result = await sender.Send(addToFolderCommand, cancellationToken);

            if (result.Status != HttpStatusCode.OK)
            {
                return Results.BadRequest(result);
            }

            return Results.Ok(result);
        }
    }
}
