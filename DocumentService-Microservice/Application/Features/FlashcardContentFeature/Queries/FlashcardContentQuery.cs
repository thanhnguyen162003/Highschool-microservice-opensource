using Algolia.Search.Models.Search;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardContentModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.FlashcardContentFeature.Queries;

public record FlashcardContentQuery : IRequest<PagedList<FlashcardContentResponseModel>>
{
    public FlashcardQueryFilter QueryFilter;
    public Guid FlashcardId;
}

public class FlashcardContentQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions,
    IClaimInterface claim)
    : IRequestHandler<FlashcardContentQuery, PagedList<FlashcardContentResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<FlashcardContentResponseModel>> Handle(FlashcardContentQuery request, CancellationToken cancellationToken)
    {
        var user = claim.GetCurrentUserId;
        if (user != Guid.Empty)
        {
            var flashcard = await unitOfWork.FlashcardContentRepository.GetFlashcardContentStarred(request.QueryFilter,request.FlashcardId,user);
            if (!flashcard.Any())
            {
                return new PagedList<FlashcardContentResponseModel>(new List<FlashcardContentResponseModel>(), 0, 0, 0);
            }
            var result = mapper.Map<List<FlashcardContentResponseModel>>(flashcard);
            foreach (var item in result)
            {
                var check = flashcard.FirstOrDefault(x => x.Id == item.Id);
                if (check is not null)
                {
                    if (check.StarredTerm.Count > 0)
                    {
                        item.IsStarred = check.StarredTerm.Any(x => x.UserId == user);
                    }
                    if (check.StudiableTerm.Count > 0)
                    {
                        item.IsLearned = check.StudiableTerm.Any(x => x.UserId == user);
                    }
                }
            }
            return PagedList<FlashcardContentResponseModel>.Create(result, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        }
        else
        {
            var listFlashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContent(request.QueryFilter, request.FlashcardId);
        
            if (!listFlashcardContent.Any())
            {
                return new PagedList<FlashcardContentResponseModel>(new List<FlashcardContentResponseModel>(), 0, 0, 0);
            }
            var mapperList = mapper.Map<List<FlashcardContentResponseModel>>(listFlashcardContent);
            
            return PagedList<FlashcardContentResponseModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);

        }
            
    }
}
