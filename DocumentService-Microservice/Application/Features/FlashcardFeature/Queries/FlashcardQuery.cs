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

public record FlashcardQuery : IRequest<PagedList<FlashcardModel>>
{
    public required FlashcardQueryFilter QueryFilter;
}

public class FlashcardQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions,
    IClaimInterface claimInterface)
    : IRequestHandler<FlashcardQuery, PagedList<FlashcardModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<FlashcardModel>> Handle(FlashcardQuery request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        IEnumerable<Flashcard> listFlashcard;

        // Chuyển đổi an toàn từ array sang list cho Tags
        List<string>? tagsList = request.QueryFilter.Tags?.ToList();

        if (!string.IsNullOrEmpty(request.QueryFilter.Search))
        {
            // Sử dụng phương thức tìm kiếm full-text với hỗ trợ lọc theo tags
            listFlashcard = await unitOfWork.FlashcardRepository.SearchFlashcardsFullText(
                request.QueryFilter.Search,
                request.QueryFilter.PageNumber,
                request.QueryFilter.PageSize,
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
            // Sử dụng các phương thức thông thường với hỗ trợ lọc theo tags
            if (userId != Guid.Empty)
            {
                listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcardsWithToken(request.QueryFilter, userId);
            }
            else
            {
                listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcards(request.QueryFilter);
            }

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
