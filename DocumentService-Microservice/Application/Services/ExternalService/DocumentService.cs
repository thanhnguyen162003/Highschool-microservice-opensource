using Domain.Entities;
using Grpc.Core;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.ExternalService
{
    public class DocumentService(IUnitOfWork unitOfWork) : DocumentServiceRpc.DocumentServiceRpcBase
    {
        public override async Task<DocumentResponse> GetDocumentId(DocumentRequest request, ServerCallContext context)
        {
            var subjectCurriculumId = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumIdBySubjectId(request.SubjectId);
            var document = await unitOfWork.DocumentRepository.GetDocumentBySubjectId(subjectCurriculumId);

            var response = new DocumentResponse();
            response.DocumentId.AddRange(document);
            return response;
        }
        public override async Task<DocumentTipsResponse> GetDocumentTips(DocumentTipsRequest request, ServerCallContext context)
        {
            var document = await unitOfWork.DocumentRepository.GetDocumentByIds(request.DocumentId).ConfigureAwait(false);
            if (document == null || !document.Any())
            {
                return new DocumentTipsResponse(); // Return an empty response if no data is found.
            }
            var response = new DocumentTipsResponse
            {
                DocumentId = { document.Select(f => f.Id.ToString()) },
                DocumentName = { document.Select(f => f.DocumentName) },
                DocumentSlug = { document.Select(f => f.Slug) }
            };
            return response;
        }
    }
}
