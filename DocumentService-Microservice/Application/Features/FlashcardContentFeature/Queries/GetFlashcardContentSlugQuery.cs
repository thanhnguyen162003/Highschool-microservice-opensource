using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardContentModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

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
        var listFlashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContent(request.QueryFilter, flashcard.Id);
        
        if (!listFlashcardContent.Any())
        {
            return new PagedList<FlashcardContentResponseModel>(new List<FlashcardContentResponseModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<FlashcardContentResponseModel>>(listFlashcardContent);
        return PagedList<FlashcardContentResponseModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}
