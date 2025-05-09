using Application.Common.Models.TagModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.TagFeature.Queries;

public record GetPopularTagsQuery : IRequest<List<TagResponseModel>>
{
    public int Limit { get; set; } = 10;
}

public class GetPopularTagsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPopularTagsQuery, List<TagResponseModel>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<List<TagResponseModel>> Handle(GetPopularTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _unitOfWork.TagRepository.GetPopularTagsAsync(request.Limit, cancellationToken);
        return _mapper.Map<List<TagResponseModel>>(tags);
    }
}