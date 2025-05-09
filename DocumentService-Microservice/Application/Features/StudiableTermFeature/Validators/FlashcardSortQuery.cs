using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardContentModel;
using Domain.CustomEntities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.StudiableTermFeature.Queries;

public record FlashcardSortQuery : IRequest<List<FlashcardContentResponseModel>>
{
    public Guid FlashcardId;
}

public class FlashcardSortQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions,
    IClaimInterface claimInterface)
    : IRequestHandler<FlashcardSortQuery, List<FlashcardContentResponseModel>>
{

    public async Task<List<FlashcardContentResponseModel>> Handle(FlashcardSortQuery request, CancellationToken cancellationToken)
    {
        if (claimInterface.GetCurrentUserId == Guid.Empty)
        {
            return new List<FlashcardContentResponseModel>();
        }
        var listFlashcardContent = await unitOfWork.FlashcardContentRepository.GetFlashcardContentSort(request.FlashcardId, claimInterface.GetCurrentUserId);
        var mapperList = mapper.Map<List<FlashcardContentResponseModel>>(listFlashcardContent);
        return mapperList;
    }
}
