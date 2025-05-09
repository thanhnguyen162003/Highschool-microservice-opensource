using Application.Common.Models.DaprModel.Document;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.DaprService.Document
{
    public record DaprCheckResourceExists : IRequest<CheckResourceExistsResponseDapr>
    {
        public CheckResourceExistsRequestDapr Check { get; set; }
    }
    public class DaprCheckResourceExistsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<DaprCheckResourceExists, CheckResourceExistsResponseDapr>
    {
        public async Task<CheckResourceExistsResponseDapr> Handle(DaprCheckResourceExists request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.Check.ResourceId, out var resourceId))
            {
                return new CheckResourceExistsResponseDapr
                {
                    Result = new ResourceCheckResultDapr
                    {
                        ResourceId = request.Check.ResourceId,
                        Exists = false,
                        ResourceType = ""
                    }
                };
            }

            string requestedResourceType = request.Check.ResourceType;
            object resource = null;

            switch (requestedResourceType)
            {
                case "Flashcard":
                    resource = await unitOfWork.FlashcardRepository.GetFlashcardById(resourceId);
                    break;
                case "FlashcardContent":
                    resource = await unitOfWork.FlashcardContentRepository.GetFlashcardContentById(resourceId);
                    break;
                case "Subject":
                    resource = await unitOfWork.SubjectRepository.GetSubjectBySubjectId(resourceId);
                    break;
                case "Document":
                    resource = await unitOfWork.DocumentRepository.GetByIdAsync(resourceId, cancellationToken);
                    break;
                default:
                    return new CheckResourceExistsResponseDapr
                    {
                        Result = new ResourceCheckResultDapr
                        {
                            ResourceId = request.Check.ResourceId,
                            Exists = false,
                            ResourceType = "Unknown"
                        }
                    };
            }

            if (resource == null)
            {
                return new CheckResourceExistsResponseDapr
                {
                    Result = new ResourceCheckResultDapr
                    {
                        ResourceId = request.Check.ResourceId,
                        Exists = false,
                        ResourceType = requestedResourceType
                    }
                };
            }

            return new CheckResourceExistsResponseDapr
            {
                Result = new ResourceCheckResultDapr
                {
                    ResourceId = request.Check.ResourceId,
                    Exists = true,
                    ResourceType = requestedResourceType
                }
            };
        }
    }
}
