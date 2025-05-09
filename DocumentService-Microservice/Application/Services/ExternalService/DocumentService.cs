//using Domain.Entities;
//using Grpc.Core;
//using Infrastructure.Repositories.Interfaces;

//namespace Application.Services.ExternalService
//{
//    public class DocumentService(IUnitOfWork unitOfWork) : DocumentServiceRpc.DocumentServiceRpcBase
//    {
//        public override async Task<DocumentResponse> GetDocumentId(DocumentRequest request, ServerCallContext context)
//        {
//            var subjectCurriculumId = await unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculumIdBySubjectId(request.SubjectId);
//            var document = await unitOfWork.DocumentRepository.GetDocumentBySubjectId(subjectCurriculumId);

//            var response = new DocumentResponse();
//            response.DocumentId.AddRange(document);
//            return response;
//        }
//        public override async Task<DocumentTipsResponse> GetDocumentTips(DocumentTipsRequest request, ServerCallContext context)
//        {
//            var document = await unitOfWork.DocumentRepository.GetDocumentByIds(request.DocumentId).ConfigureAwait(false);
//            if (document == null || !document.Any())
//            {
//                return new DocumentTipsResponse(); // Return an empty response if no data is found.
//            }
//            var response = new DocumentTipsResponse
//            {
//                DocumentId = { document.Select(f => f.Id.ToString()) },
//                DocumentName = { document.Select(f => f.DocumentName) },
//                DocumentSlug = { document.Select(f => f.Slug) }
//            };
//            return response;
//        }
        
//        public override async Task<CheckResourceExistsResponse> CheckResourceExists(
//            CheckResourceExistsRequest request, ServerCallContext context)
//        {
//            if (!Guid.TryParse(request.ResourceId, out var resourceId))
//            {
//                return new CheckResourceExistsResponse
//                {
//                    Result = new ResourceCheckResult
//                    {
//                        ResourceId = request.ResourceId,
//                        Exists = false,
//                        ResourceType = ""
//                    }
//                };
//            }

//            string requestedResourceType = request.ResourceType;
//            object resource = null;

//            switch (requestedResourceType)
//            {
//                case "Flashcard":
//                    resource = await unitOfWork.FlashcardRepository.GetFlashcardById(resourceId);
//                    break;
//                case "FlashcardContent":
//                    resource = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(resourceId);
//                    break;
//                case "Subject":
//                    resource = await unitOfWork.SubjectRepository.GetSubjectBySubjectId(resourceId);
//                    break;
//                case "Document":
//                    resource = await unitOfWork.DocumentRepository.GetByIdAsync(resourceId, context.CancellationToken);
//                    break;
//                default:
//                    return new CheckResourceExistsResponse
//                    {
//                        Result = new ResourceCheckResult
//                        {
//                            ResourceId = request.ResourceId,
//                            Exists = false,
//                            ResourceType = "Unknown"
//                        }
//                    };
//            }

//            if (resource == null)
//            {
//                return new CheckResourceExistsResponse
//                {
//                    Result = new ResourceCheckResult
//                    {
//                        ResourceId = request.ResourceId,
//                        Exists = false,
//                        ResourceType = requestedResourceType
//                    }
//                };
//            }

//            return new CheckResourceExistsResponse
//            {
//                Result = new ResourceCheckResult
//                {
//                    ResourceId = request.ResourceId,
//                    Exists = true,
//                    ResourceType = requestedResourceType
//                }
//            };
//        }
//    }
//}
