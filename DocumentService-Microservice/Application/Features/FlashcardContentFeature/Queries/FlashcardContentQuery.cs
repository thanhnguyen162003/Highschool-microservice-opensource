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
    IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<FlashcardContentQuery, PagedList<FlashcardContentResponseModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<FlashcardContentResponseModel>> Handle(FlashcardContentQuery request, CancellationToken cancellationToken)
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
