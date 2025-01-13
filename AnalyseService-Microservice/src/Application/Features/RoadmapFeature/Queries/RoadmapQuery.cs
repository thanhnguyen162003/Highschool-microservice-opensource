using Application.Common.Models.RoadmapDataModel;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Application.Features.RoadmapFeature.Queries;

public record RoadmapQuery : IRequest<PagedList<RoadmapResponseModel>>
{
    public RoadmapQueryFilter QueryFilter { get; init; }
}
public class RoadmapQueryHandler(
    IMapper mapper,
    AnalyseDbContext dbContext,
    IOptions<PaginationOptions> paginationOptions,
    ILogger<RoadmapQueryHandler> logger)
    : IRequestHandler<RoadmapQuery, PagedList<RoadmapResponseModel>>
{
    public async Task<PagedList<RoadmapResponseModel>> Handle(RoadmapQuery request, CancellationToken cancellationToken)
    {
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? paginationOptions.Value.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? paginationOptions.Value.DefaultPageSize : request.QueryFilter.PageSize;
        var listRoadmap = await dbContext.Roadmap.Find(x=> true)
            .SortBy(o=>o.CreatedAt)
            .Skip((request.QueryFilter.PageNumber - 1) * request.QueryFilter.PageSize).Limit(request.QueryFilter.PageSize)
            .ToListAsync();
        if (!listRoadmap.Any())
        {
            return new PagedList<RoadmapResponseModel>(new List<RoadmapResponseModel>(), 0, 0, 0);
        }
        var mapperList = mapper.Map<List<RoadmapResponseModel>>(listRoadmap);
        return PagedList<RoadmapResponseModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}
