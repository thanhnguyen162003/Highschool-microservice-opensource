using Application.Common.Interfaces.AWS3ServiceInterface;
using Application.Common.Models.DocumentModel;
using Application.Common.Ultils;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.DocumentFeature.Queries;

public record DownloadDocumentFileQuery : IRequest<DocumentDownloadResponseModel>
{
    public Guid DocumentId;
}
public class DownloadDocumentFileQueryHandler(MediaDbContext dbContext, IAWSS3Service aWSS3Service, IMapper mapper) : IRequestHandler<DownloadDocumentFileQuery, DocumentDownloadResponseModel>
{
    public async Task<DocumentDownloadResponseModel> Handle(DownloadDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var updateDocument = await dbContext.DocumentFiles
                            .Find(documentFile => documentFile.DocumentId == request.DocumentId)
                            .FirstOrDefaultAsync();
        var key = Uri.UnescapeDataString(UrlHelper.GetS3KeyFromUrl(updateDocument.DocumentFileUrl));
        var download = await aWSS3Service.DownloadFileAsync(key);
        return new DocumentDownloadResponseModel()
        {
            DocumentFile = download,
            DocumentName = Path.GetFileName(key)
        };
    }
}
