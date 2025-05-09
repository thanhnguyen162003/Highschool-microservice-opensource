using Application.Common.Models.DocumentModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Constants;
using Application.MaintainData.KafkaMessageModel;

namespace Application.Features.DocumentFeature.Queries
{
    public record GetDocumentBySlugManagementQuery : IRequest<DocumentManagementResponseModel>
    {
        public string DocumentSlug;
    }
    public class GetDocumentBySlugQueryManagementHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claimInterface,
        ILogger<GetDocumentBySlugQueryManagementHandler> logger, IProducerService producerService) : IRequestHandler<GetDocumentBySlugManagementQuery, DocumentManagementResponseModel>
    {
        public async Task<DocumentManagementResponseModel> Handle(GetDocumentBySlugManagementQuery request, CancellationToken cancellationToken)
        {
            if (claimInterface.GetRole != "Admin" && claimInterface.GetRole != "Moderator")
            {
                return null;
            }

            Expression<Func<Document, object>>[] documentIncludeProperties =
            {
                document => document.SubjectCurriculum,
                document => document.School,
                document => document.SubjectCurriculum.Subject,
                document => document.SubjectCurriculum.Subject.MasterSubject,
            };

            var document = await unitOfWork.DocumentRepository.Get(
                filter: document => document.Slug! == request.DocumentSlug && document.DeletedAt == null,
                includeProperties: documentIncludeProperties);

            var documentView = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DocumentViewUpdate, document.FirstOrDefault().Id.ToString(), document.FirstOrDefault().Id.ToString());
            if (claimInterface.GetCurrentUserId != Guid.Empty)
            {
                RecentViewModel recentView = new RecentViewModel()
                {
                    UserId = claimInterface.GetCurrentUserId,
                    IdDocument = document.FirstOrDefault().Id,
                    SlugDocument = document.FirstOrDefault().Slug,
                    TypeDocument = TypeDocumentConstaints.Document,
                    DocumentName = document.FirstOrDefault().DocumentName,
                    Time = DateTime.UtcNow
                };
                _ = Task.Run(()
                    => producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecentViewCreated,
                        claimInterface.GetCurrentUserId.ToString(), recentView), cancellationToken);
                var map = mapper.Map<DocumentManagementResponseModel>(document.FirstOrDefault());
                var userLike = await unitOfWork.UserLikeRepository.GetUserLikeDocumentAsync(claimInterface.GetCurrentUserId, document.FirstOrDefault().Id);
                if (userLike != null)
                {
                    if (userLike.DocumentId == document.FirstOrDefault().Id)
                    {
                        map.IsLike = true;
                    }
                    else
                    {
                        map.IsLike = false;
                    }
                }
                return map;
            }

            return mapper.Map<DocumentManagementResponseModel>(document.FirstOrDefault());
        }
    }
}
