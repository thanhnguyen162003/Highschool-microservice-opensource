using Application.Common.Models.TagModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TagFeature.Queries;

public record GetFlashcardTagsQuery : IRequest<List<TagResponseModel>>
{
    public Guid FlashcardId { get; set; }
}

public class GetFlashcardTagsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetFlashcardTagsQuery, List<TagResponseModel>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<List<TagResponseModel>> Handle(GetFlashcardTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _unitOfWork.TagRepository.GetTagsByFlashcardIdAsync(request.FlashcardId, cancellationToken);
        return _mapper.Map<List<TagResponseModel>>(tags);
    }
}