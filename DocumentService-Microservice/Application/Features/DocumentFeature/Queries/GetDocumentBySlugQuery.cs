using Application.Common.Models.DocumentModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Constants;
using Application.KafkaMessageModel;
using Application.MaintainData.KafkaMessageModel;

namespace Application.Features.DocumentFeature.Queries
{
    public record GetDocumentBySlugQuery : IRequest<DocumentResponseModel>
    {
        public string DocumentSlug;
    }
    public class GetDocumentBySlugQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claimInterface,
        ILogger<GetDocumentBySlugQueryHandler> logger, IProducerService producerService) : IRequestHandler<GetDocumentBySlugQuery, DocumentResponseModel>
    {
        public async Task<DocumentResponseModel> Handle(GetDocumentBySlugQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Document, object>>[] documentIncludeProperties =
            {
                document => document.SubjectCurriculum,
                document => document.SubjectCurriculum.Subject,
                document => document.SubjectCurriculum.Subject.MasterSubject,
            };
            var document = await unitOfWork.DocumentRepository.Get(
                filter: document => document.Slug! == request.DocumentSlug && document.DeletedAt == null,
                includeProperties: documentIncludeProperties);

            var documentView = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DocumentViewUpdate, document.FirstOrDefault().Id.ToString(), document.FirstOrDefault().Id.ToString());
            if (claimInterface.GetCurrentUserId != Guid.Empty)
            {
                // UserAnalyseMessageModel dataModel = new UserAnalyseMessageModel()
                // {
                //     UserId = claimInterface.GetCurrentUserId,
                //     SubjectId = document.FirstOrDefault().SubjectCurriculum.SubjectId,
                //     DocumentId = document.FirstOrDefault().Id
                // };
                // var result = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserAnalyseData,claimInterface.GetCurrentUserId.ToString(), dataModel);
                // if (result is false)
                // {
                //     logger.LogError($"User {claimInterface.GetCurrentUserId} was not track by system");
                // }
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
                var map = mapper.Map<DocumentResponseModel>(document.FirstOrDefault());
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
            
            return mapper.Map<DocumentResponseModel>(document.FirstOrDefault());
        }
    }
}
