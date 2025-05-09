using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.UUID;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Application.Common.Helpers;
using Application.MaintainData.KafkaMessageModel;

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
        if (flashcard is null)
        {
	        return new FlashcardModel();
        }
		if (claimService.GetCurrentUserId != Guid.Empty)
        {
			var containerUser = await unitOfWork.ContainerRepository.GetContainerByUserId(userId, flashcard.Id, cancellationToken);
			if (containerUser is null)
			{
				// Create container for user.
				Container newContainer = new Container()
				{
					Id = new UuidV7().Value,
					FlashcardId = flashcard.Id,
					UserId = userId,
					ViewAt = DateTime.UtcNow,
					ShuffleFlashcards = false,
					LearnRound = 0,
					LearnMode = LearnMode.Learn,
					ShuffleLearn = false,
					StudyStarred = false,
					AnswerWith = StudySetAnswerMode.Definition,
					MultipleAnswerMode = MultipleAnswerMode.Unknown,
					ExtendedFeedbackBank = false,
					EnableCardsSorting = false,
					CardsRound = 0,
					CardsStudyStarred = false,
					CardsAnswerWith = LimitedStudySetAnswerMode.Definition,
					MatchStudyStarred = false,
                    FsrsParameters = new double[19] { 0.40255, 1.18385, 3.173, 15.69105, 7.1949, 0.5345, 1.4604, 0.0046,
                        1.54575, 0.1192, 1.01925, 1.9395, 0.11, 0.29605, 2.2698, 0.2315,
                        2.9898, 0.51655, 0.6621},
                    Retrievability = 0.9,
                    CardsPerDay = 20
                };
				var result = await unitOfWork.ContainerRepository.CreateContainer(newContainer, cancellationToken);
				if (result is false)
				{
					logger.LogError("Create container for user failed");
					return new FlashcardModel();
				}
				containerUser = newContainer;
			}

            if (flashcard.Created == true)
            {
                RecentViewModel recentView = new RecentViewModel()
                {
                    UserId = claimService.GetCurrentUserId,
                    IdDocument = flashcard.Id,
                    SlugDocument = flashcard.Slug,
                    TypeDocument = TypeDocumentConstaints.Flashcard,
                    DocumentName = flashcard.FlashcardName,
                    Time = DateTime.UtcNow
                };

                await producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecentViewCreated, claimService.GetCurrentUserId.ToString(), recentView);
            }

            var studiableTerms = await unitOfWork.StudiableTermRepository.GetStudiableTerm(userId, containerUser.Id);
            var starredTerm = await unitOfWork.StarredTermRepository.GetStarredTerm(userId, containerUser.Id);
            List<Guid> combinedList = starredTerm.Select(x => x.FlashcardContentId).ToList();
            combinedList.AddRange(studiableTerms.Select(x => x.FlashcardContentId).Where(x => x.HasValue).Select(x => x.Value));

            var flashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentByIds(combinedList);
            for (int i = 0; i < starredTerm.Count; i++)
            {
                starredTerm[i].FlashcardContent = flashcardContent.FirstOrDefault(x => x.Id.Equals(starredTerm[i].FlashcardContentId));
            }
            for (int i = 0; i < studiableTerms.Count; i++)
            {
                studiableTerms[i].FlashcardContent = flashcardContent.FirstOrDefault(x => x.Id.Equals(studiableTerms[i].FlashcardContentId));
            }
            containerUser.StarredTerms = starredTerm;
            containerUser.StudiableTerms = studiableTerms;

            var map = mapper.Map<FlashcardModel>(flashcard);
			map.Container = containerUser;
            
            // Sử dụng helper để tải các đối tượng liên quan
            await FlashcardHelper.LoadRelatedEntities(map, flashcard, unitOfWork);
            
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
            
            var preset = await unitOfWork.FSRSPresetRepository.Get(preset => containerUser.Retrievability == preset.Retrievability && containerUser.FsrsParameters.Equals(preset.FsrsParameters));

            map.PresetId = preset.FirstOrDefault()?.Id ?? null;
            
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
            
            var map = mapper.Map<FlashcardModel>(flashcard);
            await FlashcardHelper.LoadRelatedEntities(map, flashcard, unitOfWork);
            return map;
        }
        return new FlashcardModel();
    }
}