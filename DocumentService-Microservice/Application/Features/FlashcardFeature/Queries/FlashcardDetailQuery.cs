using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Queries;

public record FlashcardDetailQuery : IRequest<FlashcardModel>
{
    public Guid flashcardId;
}

public class FlashcardDetailQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface,
    IProducerService producerService,
    ILogger<FlashcardDetailQueryHandler> logger)
    : IRequestHandler<FlashcardDetailQuery, FlashcardModel>
{
    public async Task<FlashcardModel> Handle(FlashcardDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        Flashcard flashcard = new Flashcard();
        if (userId == Guid.Empty)
        {
           flashcard = await unitOfWork.FlashcardRepository.GetFlashcardById(request.flashcardId);
        }
        if (userId != Guid.Empty)
        {
            flashcard = await unitOfWork.FlashcardRepository.GetFlashcardByIdWithToken(request.flashcardId, userId);
            if(flashcard is null)
            {
                return new FlashcardModel();
            }
            UserAnalyseMessageModel dataModel = new UserAnalyseMessageModel()
            {
                UserId = claimInterface.GetCurrentUserId,
                SubjectId = flashcard.SubjectId,
                FlashcardId = flashcard.Id
            };
            _ = Task.Run(async () =>
            {
                var result = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserAnalyseData, claimInterface.GetCurrentUserId.ToString(), dataModel);
                if (result is false)
                {
                    logger.LogError($"User {claimInterface.GetCurrentUserId} was not track by system");
                }
            }, cancellationToken);
            RecentViewModel recentView = new RecentViewModel()
            {
                UserId = claimInterface.GetCurrentUserId,
                IdDocument = flashcard.Id,
                SlugDocument = flashcard.Slug,
                TypeDocument = TypeDocumentConstaints.Flashcard,
                DocumentName = flashcard.FlashcardName,
                Time = DateTime.Now
            };
            _ = Task.Run(() => producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecentViewCreated, claimInterface.GetCurrentUserId.ToString(), recentView), cancellationToken);
            var map = mapper.Map<FlashcardModel>(flashcard);
            var userLike = await unitOfWork.UserLikeRepository.GetUserLikeFlashcardAsync(userId, request.flashcardId);
            if (userLike != null) 
            {
                if (userLike.FlashcardId == flashcard.Id) 
                {
                    map.IsRated = true;
                }
                else
                {
                    map.IsRated = false;
                }
            }
            if (flashcard is not null)
            {
                _ = Task.Run(async () =>
                {
                    var flashcardView = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.FlashcardViewUpdate, flashcard.Id.ToString(), flashcard.Id.ToString());
                    if (flashcardView is false)
                    {
                        logger.LogCritical("Kafka view flashcard lỗi");
                    }
                }, cancellationToken);
            }
            return map;
        }
        if (flashcard is not null)
        {
            _ = Task.Run(async () =>
            {
                var flashcardView = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.FlashcardViewUpdate, flashcard.Id.ToString(), flashcard.Id.ToString());
                if (flashcardView is false)
                {
                    logger.LogCritical("Kafka view flashcard lỗi");
                }
            }, cancellationToken);
        }
        return mapper.Map<FlashcardModel>(flashcard);
    }
}