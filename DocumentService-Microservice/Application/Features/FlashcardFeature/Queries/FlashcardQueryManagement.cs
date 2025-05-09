using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Application.Common.Helpers;

namespace Application.Features.FlashcardFeature.Queries;

public record FlashcardQueryManagement : IRequest<PagedList<FlashcardModel>>
{
    public FlashcardQueryFilterManagement QueryFilter;
}

public class FlashcardQueryManagementHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions,
    IClaimInterface claimInterface)
    : IRequestHandler<FlashcardQueryManagement, PagedList<FlashcardModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<FlashcardModel>> Handle(FlashcardQueryManagement request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        var isAdmin = claimInterface.GetRole == "Admin" || claimInterface.GetRole == "Moderator";
        if (!isAdmin)
        {
            return null;
        }
        IEnumerable<Flashcard> listFlashcard;

        List<string>? tagsList = request.QueryFilter.Tags?.ToList();

        if (!string.IsNullOrEmpty(request.QueryFilter.Search))
        {
            listFlashcard = await unitOfWork.FlashcardRepository.SearchFlashcardsFullTextManagement(
                request.QueryFilter.Search,
                request.QueryFilter.PageNumber,
                request.QueryFilter.PageSize,
                request.QueryFilter.UserId,
                request.QueryFilter.IsCreatedBySystem,
                request.QueryFilter.Status,
                request.QueryFilter.IsDeleted,
                tagsList,
                cancellationToken);

            // Sử dụng FlashcardHelper để lọc theo các thuộc tính mới
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
            listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcardsManagement(request.QueryFilter);

            // Sử dụng FlashcardHelper để lọc theo các thuộc tính mới
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
            return new PagedList<FlashcardModel>(new List<FlashcardModel>(), 0, 0, 0);
        }

        var mapperList = mapper.Map<List<FlashcardModel>>(listFlashcard);

        // Đảm bảo tất cả flashcard đều có tags (nếu cần)
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

        // Tải thông tin các đối tượng liên quan cho danh sách flashcard
        await FlashcardHelper.LoadRelatedEntitiesForList(mapperList, unitOfWork);

        if (!string.IsNullOrEmpty(request.QueryFilter.Search))
        {
            int totalCount = (request.QueryFilter.PageNumber - 1) * request.QueryFilter.PageSize + mapperList.Count;
            var pagedList = new PagedList<FlashcardModel>(
                mapperList,
                totalCount,
                request.QueryFilter.PageNumber,
                request.QueryFilter.PageSize);

            return pagedList;
        }
        else
        {
            var pagedList = PagedList<FlashcardModel>.Create(
                mapperList,
                request.QueryFilter.PageNumber,
                request.QueryFilter.PageSize);

            return pagedList;
        }
    }
}
