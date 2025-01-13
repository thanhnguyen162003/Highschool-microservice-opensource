using Domain.Entities;
using Grpc.Core;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services.ExternalService
{
    public class DocumentService : DocumentServiceRpc.DocumentServiceRpcBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocumentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<DocumentResponse> GetDocumentId(DocumentRequest request, ServerCallContext context)
        {
            var subjectCurriculumId = await _unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumIdBySubjectId(request.SubjectId);
            var document = await _unitOfWork.DocumentRepository.GetDocumentBySubjectId(subjectCurriculumId);

            var response = new DocumentResponse();
            response.DocumentId.AddRange(document);
            return response;
        }
        public override async Task<DocumentTipsResponse> GetDocumentTips(DocumentTipsRequest request, ServerCallContext context)
        {
            var document = await _unitOfWork.DocumentRepository.GetDocumentByIds(request.DocumentId);

            var response = new DocumentTipsResponse();
            foreach (var item in document)
            {
                response.DocumentId.Add(item.Id.ToString());
                response.DocumentName.Add(item.DocumentName.ToString());
                response.DocumentSlug.Add(item.Slug.ToString());
            }
            return response;
        }
    }
}
