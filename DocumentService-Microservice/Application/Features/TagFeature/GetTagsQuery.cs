using Application.Common.Models.TagModel;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.TagFeature.Queries;

public record GetTagsQuery : IRequest<PagedList<TagResponseModel>>
{
    public TagQueryFilter QueryFilter { get; set; } = new TagQueryFilter();
}

public class GetTagsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions) : IRequestHandler<GetTagsQuery, PagedList<TagResponseModel>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<TagResponseModel>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _unitOfWork.TagRepository.GetTagsPaginated(
            request.QueryFilter.Search,
            request.QueryFilter.PageNumber,
            request.QueryFilter.PageSize,
            cancellationToken);

        var count = await _unitOfWork.TagRepository.CountTagsAsync(request.QueryFilter.Search, cancellationToken);

        var tagModels = _mapper.Map<List<TagResponseModel>>(tags);

        return new PagedList<TagResponseModel>(
            tagModels,
            count,
            request.QueryFilter.PageNumber,
            request.QueryFilter.PageSize);
    }
}