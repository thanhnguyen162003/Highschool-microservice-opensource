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
    public record GetDocumentByIdQuery : IRequest<DocumentResponseModel>
    {
        public Guid DocumentId;
    }
    public class GetDocumentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claimInterface,
        IProducerService producerService, ILogger<GetDocumentByIdQueryHandler> logger) : IRequestHandler<GetDocumentByIdQuery, DocumentResponseModel>
    {
        public async Task<DocumentResponseModel> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Document, object>>[] documentIncludeProperties =
            {
                document => document.SubjectCurriculum,
                document => document.SubjectCurriculum.Subject,
                document => document.SubjectCurriculum.Subject.MasterSubject,
            };
            var document = await unitOfWork.DocumentRepository.Get(filter: document => document.Id == request.DocumentId && document.DeletedAt == null,
                                                                   includeProperties: documentIncludeProperties);
            //if (!document.IsNullOrEmpty())
            //{
            //    var updateDocument = document.FirstOrDefault();
            //    updateDocument!.View++;
            //    _ = Task.Run(async () =>
            //    {
            //        var _ = unitOfWork.DocumentRepository.Update(updateDocument);
            //        await unitOfWork.SaveChangesAsync();
            //    });
            //}
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
                var userLike = await unitOfWork.UserLikeRepository.GetUserLikeDocumentAsync(claimInterface.GetCurrentUserId, request.DocumentId);
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
