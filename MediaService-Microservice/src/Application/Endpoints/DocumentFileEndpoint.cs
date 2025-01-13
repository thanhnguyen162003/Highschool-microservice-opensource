using System.ComponentModel.DataAnnotations;
using Application.Common.Ultils;
using Application.Features.DocumentFeature.Commands;
using Application.Features.DocumentFeature.Queries;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public class DocumentFileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("document/{documentId}", GetDocumentFileById).WithName(nameof(GetDocumentFileById));
        group.MapGet("document/download/{documentId}", DownloadDocumentFileById).WithName(nameof(DownloadDocumentFileById));
        group.MapPost("document/{documentId}", UploadDocumentFileById).DisableAntiforgery().RequireAuthorization().WithName(nameof(UploadDocumentFileById));
    }

    public static async Task<IResult> GetDocumentFileById([Required] Guid documentId, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetDocumentFileQuery()
        {
            DocumentId = documentId,
        };
        var result = await sender.Send(query, cancellationToken);
        return JsonHelper.Json(result);
    }

    public static async Task<IResult> UploadDocumentFileById([FromForm] IFormFile file, [Required] Guid documentId,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new UploadDocumentFileCommand()
        {
            DocumentId = documentId,
            DocumentFile = file,
        };
        var result = await sender.Send(command, cancellationToken);
        return JsonHelper.Json(result);
    }

    public static async Task<IResult> DownloadDocumentFileById([Required] Guid documentId, ISender sender, CancellationToken cancellationToken)
    {
        var query = new DownloadDocumentFileQuery()
        {
            DocumentId = documentId
        };
        var result = await sender.Send(query, cancellationToken);
        return Results.File(result.DocumentFile!, "application/pdf", result.DocumentName);
    }
}
