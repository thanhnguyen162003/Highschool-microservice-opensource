using System.Net;
using Application.Common.Models.RoadmapDataModel;
using Application.Common.Models.SearchModel;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;

namespace Application.Features.RoadmapFeature.Commands;

public record CreateRoadmapCommand : IRequest<ResponseModel>
{
    public RoadmapCreateRequestModel RoadmapCreateRequestModel;
}

public class CreateRoadmapCommandHandler(
    IMapper mapper,
    AnalyseDbContext dbContext,
    ILogger<CreateRoadmapCommandHandler> logger)
    : IRequestHandler<CreateRoadmapCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateRoadmapCommand request, CancellationToken cancellationToken)
    {
        var roadmap = mapper.Map<Roadmap>(request.RoadmapCreateRequestModel);
        roadmap.Id = ObjectId.GenerateNewId().ToString();
        await dbContext.Roadmap.InsertOneAsync(roadmap, cancellationToken: cancellationToken);
        logger.LogInformation("Roadmap created successfully with id: {RoadmapId}", roadmap.Id);
        return new ResponseModel(HttpStatusCode.OK, "Roadmap created successfully", roadmap.Id);
    }
}
    