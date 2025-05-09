using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardModel;
using Application.Services;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Application.Features.FlashcardFeature.Queries
{
    public class FlashcardQueryStudy : IRequest<PagedList<FlashcardStudyResponseModel>>
    {
        public FlashcardQueryFilter QueryFilter;
    }

    public class FlashcardQueryStudyHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IOptions<PaginationOptions> paginationOptions,
        IFlashcardStudyService flashcardStudyService,
        IClaimInterface claimInterface)
    : IRequestHandler<FlashcardQueryStudy, PagedList<FlashcardStudyResponseModel>>
    {
        private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

        public async Task<PagedList<FlashcardStudyResponseModel>> Handle(FlashcardQueryStudy request, CancellationToken cancellationToken)
        {
            var userId = claimInterface.GetCurrentUserId;
            IEnumerable<Flashcard> listFlashcard;

            List<string>? tagsList = request.QueryFilter.Tags?.ToList();

            if (!string.IsNullOrEmpty(request.QueryFilter.Search))
            {
                listFlashcard = await unitOfWork.FlashcardRepository.SearchFlashcardsFullText(
                    request.QueryFilter.Search,
                    request.QueryFilter.PageNumber,
                    request.QueryFilter.PageSize,
                    tagsList,
                    cancellationToken);

                if (listFlashcard.Any() && request.QueryFilter.EntityId.HasValue && request.QueryFilter.FlashcardType.HasValue)
                {
                    listFlashcard = FlashcardHelper.FilterByEntity(
                        listFlashcard,
                        request.QueryFilter.EntityId.Value,
                        request.QueryFilter.FlashcardType.Value
                    );
                }
            }
            else
            {
                if (userId != Guid.Empty)
                {
                    listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcardsWithToken(request.QueryFilter, userId);
                }
                else
                {
                    listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcards(request.QueryFilter);
                }

                if (listFlashcard.Any() && request.QueryFilter.EntityId.HasValue && request.QueryFilter.FlashcardType.HasValue)
                {
                    listFlashcard = FlashcardHelper.FilterByEntity(
                        listFlashcard,
                        request.QueryFilter.EntityId.Value,
                        request.QueryFilter.FlashcardType.Value
                    );
                }
            }

            if (!listFlashcard.Any())
            {
                return new PagedList<FlashcardStudyResponseModel>(new List<FlashcardStudyResponseModel>(), 0, 0, 0);
            }

            var mapperList = mapper.Map<List<FlashcardStudyResponseModel>>(listFlashcard);

            List<FlashcardStudyResponseModel> removeList = new List<FlashcardStudyResponseModel>();

            foreach (var flashcard in mapperList)
            {
                var container = await unitOfWork.ContainerRepository.GetContainerByUserId(userId, flashcard.Id, cancellationToken);
                int cardsPerDay = container?.CardsPerDay ?? -1;

                bool checkIfHaveProgress = (await unitOfWork.UserFlashcardProgressRepository.Get(x => x.UserId == userId && x.FlashcardContent.FlashcardId == flashcard.Id)).FirstOrDefault() == null;

                if (cardsPerDay == -1 || checkIfHaveProgress)
                {
                    removeList.Add(flashcard);
                    continue;
                }

                var studySessionData = await unitOfWork.UserFlashcardProgressRepository.GetProgressByUser(userId, flashcard.Id);

                var studySessionData2 = await flashcardStudyService.GetDueFlashcards(userId, flashcard.Id, cardsPerDay, false);

                flashcard.NewCardCount = studySessionData2.DueCards
                    .Count(card =>
                        card.IsNew
                    );

                flashcard.CardInLearningCount = studySessionData2.DueCards.Count(card => card.IsReview);

                flashcard.DueForReview = studySessionData.Count(card => card.DueDate!.Value.Date <= DateTime.UtcNow.Date);

                if (flashcard.Tags == null || !flashcard.Tags.Any())
                {
                    var tags = await unitOfWork.TagRepository.GetTagsByFlashcardIdAsync(flashcard.Id, cancellationToken);
                    if (tags != null && tags.Any())
                    {
                        flashcard.Tags = tags.Select(t => t.Name).ToList();
                    }
                }
            }

            mapperList = mapperList.Except(removeList).ToList();

            await FlashcardHelper.LoadRelatedEntitiesForList(mapperList.Cast<FlashcardModel>().ToList(), unitOfWork);

            if (!string.IsNullOrEmpty(request.QueryFilter.Search))
            {
                int totalCount = (request.QueryFilter.PageNumber - 1) * request.QueryFilter.PageSize + mapperList.Count;
                var pagedList = new PagedList<FlashcardStudyResponseModel>(
                    mapperList,
                    totalCount,
                    request.QueryFilter.PageNumber,
                    request.QueryFilter.PageSize);

                return pagedList;
            }
            else
            {
                var pagedList = PagedList<FlashcardStudyResponseModel>.Create(
                    mapperList,
                    request.QueryFilter.PageNumber,
                    request.QueryFilter.PageSize);

                return pagedList;
            }
        }
    }
}
