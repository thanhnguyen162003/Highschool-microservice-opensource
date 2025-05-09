using Application.Common.Interfaces.ClaimInterface;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.FlashcardFeature.Queries;

public class FlashcardOwnQuery : IRequest<PagedList<FlashcardModel>>
{
    public FlashcardQueryFilter QueryFilter;
}

public class FlashcardOwnQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimInterface claimInterface,
    IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<FlashcardOwnQuery, PagedList<FlashcardModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<FlashcardModel>> Handle(FlashcardOwnQuery request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;

        // Chuyển đổi an toàn từ array sang list cho Tags
        List<string>? tagsList = request.QueryFilter.Tags?.ToList();

        var listFlashcard = await unitOfWork.FlashcardRepository.GetOwnFlashcard(request.QueryFilter, userId);

        if (!listFlashcard.Any())
        {
            return new PagedList<FlashcardModel>(new List<FlashcardModel>(), 0, 0, 0);
        }

        var mapperList = mapper.Map<List<FlashcardModel>>(listFlashcard);

        // Đảm bảo tất cả flashcard đều có tags
        foreach (var flashcard in mapperList)
        {
            if (flashcard.Tags == null || !flashcard.Tags.Any())
            {
                var tags = await unitOfWork.TagRepository.GetTagsByFlashcardIdAsync(flashcard.Id, cancellationToken);
                if (tags != null && tags.Any())
                {
                    flashcard.Tags = tags.Select(t => t.Name).ToList();
                }
            }
        }

        return PagedList<FlashcardModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}