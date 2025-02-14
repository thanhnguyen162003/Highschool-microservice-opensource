using Application.Common.Models.RoadmapDataModel;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.RoadmapFeature.Queries;

public record RoadmapDetailQuery : IRequest<RoadmapDetailResponseModel>
{
    public string RoadmapId { get; init; }
}
public class RoadmapDetailQueryHandler(
    IMapper mapper,
    AnalyseDbContext dbContext,
    ILogger<RoadmapDetailQueryHandler> logger)
    : IRequestHandler<RoadmapDetailQuery, RoadmapDetailResponseModel>
{
    public async Task<RoadmapDetailResponseModel> Handle(RoadmapDetailQuery request, CancellationToken cancellationToken)
    {
       var roadmap = dbContext.Roadmap.Find(x=>x.Id == request.RoadmapId).FirstOrDefault(cancellationToken);
       return mapper.Map<RoadmapDetailResponseModel>(roadmap);
    }
}
