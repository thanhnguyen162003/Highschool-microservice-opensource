using Application.Common.Models.DocumentModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DocumentFeature.Queries;

public record RelatedDocumentQuery : IRequest<List<DocumentResponseModel>>
{
    public Guid DocumentId { get; init; }
}
public class RelatedDocumentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<RelatedDocumentQuery, List<DocumentResponseModel>>
{
    
    public async Task<List<DocumentResponseModel>> Handle(RelatedDocumentQuery request, CancellationToken cancellationToken)
    {
        var listRelatedDocument = await unitOfWork.DocumentRepository.GetRelatedDocumentsByDocumentId(request.DocumentId, cancellationToken);
        return mapper.Map<List<DocumentResponseModel>>(listRelatedDocument);
    }
}
