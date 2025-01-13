using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.UoW;
using Application.Constants;
using Application.KafkaMessageModel;
using Application.Services;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.Queries;

public record FlashcardDetailSlugQuery : IRequest<FlashcardModel>
{
    public string Slug { get; init; }
}

public class FlashcardDetailSlugQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IProducerService producerService,
    IClaimInterface claimService,
    ILogger<FlashcardDetailSlugQueryHandler> logger)
    : IRequestHandler<FlashcardDetailSlugQuery, FlashcardModel>
{
    public async Task<FlashcardModel> Handle(FlashcardDetailSlugQuery request, CancellationToken cancellationToken)
    {
        var userId = claimService.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardBySlug(request.Slug, userId);
        if (claimService.GetCurrentUserId != Guid.Empty)
        {
            UserAnalyseMessageModel dataModel = new UserAnalyseMessageModel()
            {
                UserId = claimService.GetCurrentUserId,
                SubjectId = flashcard.SubjectId,
                FlashcardId = flashcard.Id
            };
            _ = Task.Run(async () =>
            {
                var result = await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserAnalyseData, claimService.GetCurrentUserId.ToString(), dataModel);
                if (result is false)
                {
                    logger.LogError($"User {claimService.GetCurrentUserId} was not track by system");
                }
            }, cancellationToken);
            RecentViewModel recentView = new RecentViewModel()
            {
                UserId = claimService.GetCurrentUserId,
                IdDocument = flashcard.Id,
                SlugDocument = flashcard.Slug,
                TypeDocument = TypeDocumentConstaints.Flashcard,
                DocumentName = flashcard.FlashcardName,
                Time = DateTime.Now
            };
            _ = Task.Run(() => producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecentViewCreated, claimService.GetCurrentUserId.ToString(), recentView), cancellationToken);
            var map = mapper.Map<FlashcardModel>(flashcard);
            var userLike = await unitOfWork.UserLikeRepository.GetUserLikeFlashcardAsync(userId, flashcard.Id);
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