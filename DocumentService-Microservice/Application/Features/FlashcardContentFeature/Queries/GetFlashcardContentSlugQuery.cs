using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardContentModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Application.Features.FlashcardContentFeature.Queries;

public record GetFlashcardContentSlugQuery : IRequest<IEnumerable<FlashcardContentResponseModel>>
{
    public FlashcardQueryFilter QueryFilter;
    public string Slug;
}

public class GetFlashcardContentSlugQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface,
    IFlashcardStudyService flashcardStudyService,
    IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<GetFlashcardContentSlugQuery, IEnumerable<FlashcardContentResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<IEnumerable<FlashcardContentResponseModel>> Handle(GetFlashcardContentSlugQuery request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        var flashcard = await unitOfWork.FlashcardRepository.GetFlashcardBySlug(request.Slug, userId);
        if (flashcard is null)
        {
            return new List<FlashcardContentResponseModel>();
        }

        if (userId != Guid.Empty)
        {
            var flashcardList = await unitOfWork.FlashcardContentRepository.GetFlashcardContentStarred(request.QueryFilter, flashcard.Id, userId);
            
            if (!flashcardList.Any())
            {
                return new PagedList<FlashcardContentResponseModel>(new List<FlashcardContentResponseModel>(), 0, 0, 0);
            }

            var idProgressList = (await unitOfWork.UserFlashcardProgressRepository.GetProgressByUser(userId, flashcard.Id)).ToList().Where(x => x.Rating >= 2)
                                        .Select(x => x.FlashcardContentId).ToList();

            var container = await unitOfWork.ContainerRepository.GetContainerByUserId(userId, flashcard.Id, cancellationToken);
            int cardsPerDay = 20;
            if (container != null)
            {
                cardsPerDay = container.CardsPerDay;
            }

            var dueFlashcardIdTodayList = (await flashcardStudyService.GetDueFlashcards(userId, flashcard.Id, cardsPerDay, false)).DueCards.Select(x => x.ContentId).ToList();

            var mapperList = mapper.Map<List<FlashcardContentResponseModel>>(flashcardList);

            foreach (var item in mapperList)
            {
                var check = flashcardList.FirstOrDefault(x => x.Id == item.Id);           

                if (check is not null)
                {
                    if (check.StarredTerm.Count > 0)
                    {
                        item.IsStarred = check.StarredTerm.Any(x => x.UserId == userId);
                    }
                    if (check.StudiableTerm.Count > 0)
                    {
                        item.IsLearned = check.StudiableTerm.Any(x => x.UserId == userId);
                    }                    
                }

                if (idProgressList.Contains(item.Id))
                {
                    item.CurrentState = CurrentState.Learned;
                }
                else if (dueFlashcardIdTodayList.Contains(item.Id))
                {
                    item.CurrentState = CurrentState.DueReviewToday;
                }
                else
                {
                    item.CurrentState = CurrentState.NotLearned;
                }
            }
            return PagedList<FlashcardContentResponseModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        }
        else
        {

            var listFlashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContent(request.QueryFilter, flashcard.Id);

            if (!listFlashcardContent.Any())
            {
                return new PagedList<FlashcardContentResponseModel>(new List<FlashcardContentResponseModel>(), 0, 0, 0);
            }
            var mapperList = mapper.Map<List<FlashcardContentResponseModel>>(listFlashcardContent);
            return PagedList<FlashcardContentResponseModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        }
    }
}
