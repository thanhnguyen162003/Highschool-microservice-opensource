using Application.Common.Models.DocumentModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Application.Features.DocumentFeature.Queries;

public record GetListDocumentIdsQuery : IRequest<List<DocumentResponseModel>>
{
    public List<Guid> DocumentIds;
}

public class GetListDocumentIdsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetListDocumentIdsQuery, List<DocumentResponseModel>>
{
    public async Task<List<DocumentResponseModel>> Handle(GetListDocumentIdsQuery request, CancellationToken cancellationToken)
    {
        List<DocumentResponseModel> documents = new List<DocumentResponseModel>();

        Expression<Func<Document, object>>[] documentIncludeProperties =
        {
            document => document.SubjectCurriculum,
            document => document.SubjectCurriculum.Subject,
            document => document.SubjectCurriculum.Subject.MasterSubject,
        };

        foreach (var documentId in request.DocumentIds)
        {
            var document = await unitOfWork.DocumentRepository.Get(
                filter: document => document.Id == documentId && document.DeletedAt == null,
                includeProperties: documentIncludeProperties);
            var documentResponse = mapper.Map<DocumentResponseModel>(document);
            documents.Add(documentResponse);
        }
        return documents;
    }
}

