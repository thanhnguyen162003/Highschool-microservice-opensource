using Application.Common.Interfaces.AWS3ServiceInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.DocumentModel;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.DocumentFeature.Queries;

public record GetDocumentFileQuery : IRequest<ResponseModel>
{
    public Guid DocumentId;
}
public class GetDocumentFileQueryHandler(MediaDbContext dbContext, IAWSS3Service aWSS3Service, IMapper mapper) : IRequestHandler<GetDocumentFileQuery, ResponseModel>
{
    public async Task<ResponseModel> Handle(GetDocumentFileQuery request, CancellationToken cancellationToken)
    {
        var updateDocument = await dbContext.DocumentFiles
                            .Find(documentFile => documentFile.DocumentId.Equals(request.DocumentId))
                            .FirstOrDefaultAsync();
        return new ResponseModel(System.Net.HttpStatusCode.OK, "Get successfully.", mapper.Map<DocumentFileResponseModel>(updateDocument));
    }
}
