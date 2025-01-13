using Application.Common.Interfaces.ClaimInterface;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.FlashcardFeature.Queries;

public record FlashcardQuery : IRequest<PagedList<FlashcardModel>>
{
    public FlashcardQueryFilter QueryFilter;
}

public class FlashcardQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions, IClaimInterface claimInterface)
    : IRequestHandler<FlashcardQuery, PagedList<FlashcardModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<FlashcardModel>> Handle(FlashcardQuery request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        IEnumerable<Flashcard> listFlashcard;
        if (userId != Guid.Empty)
        {
            listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcardsWithToken(request.QueryFilter, userId);
        }
        else
        {
            listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcards(request.QueryFilter);

        }
        if (!listFlashcard.Any())
        {
            return new PagedList<FlashcardModel>(new List<FlashcardModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<FlashcardModel>>(listFlashcard);
        return PagedList<FlashcardModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}
