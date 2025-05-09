using Application.Common.Interfaces.AzureInterface;
using Application.Common.Models.DocumentModel;
using Application.Common.Ultils;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.DocumentFeature.Queries;

public record DownloadDocumentFileQuery : IRequest<DocumentDownloadResponseModel>
{
    public Guid DocumentId;
}

public class DownloadDocumentFileQueryHandler(MediaDbContext dbContext) : IRequestHandler<DownloadDocumentFileQuery, DocumentDownloadResponseModel>
{
    private readonly MediaDbContext _dbContext = dbContext;

    public async Task<DocumentDownloadResponseModel> Handle(DownloadDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var updateDocument = await _dbContext.DocumentFiles
            .Find(documentFile => documentFile.DocumentId == request.DocumentId)
            .FirstOrDefaultAsync();

        if (updateDocument == null)
        {
            return new DocumentDownloadResponseModel()
            {
                DocumentFileUrl = null            };
        }
        return new DocumentDownloadResponseModel()
        {
            DocumentFileUrl = updateDocument.DocumentFileUrl
        };
    }
}
