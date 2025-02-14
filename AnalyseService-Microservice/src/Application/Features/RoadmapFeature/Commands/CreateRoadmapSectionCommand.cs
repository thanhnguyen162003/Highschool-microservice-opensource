using System.Net;
using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Application.Features.RoadmapFeature.Commands;

public record RoadmapDetailCreateCommand : IRequest<ResponseModel>
{
    public required RoadMapSectionCreateRequestModel RoadMapSectionCreateCommand;
}
public class RoadmapDetailCreateCommandHandler(
    AnalyseDbContext dbContext,
    IConfiguration configuration,
    ILogger<RoadmapDetailCreateCommandHandler> logger)
    : IRequestHandler<RoadmapDetailCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(RoadmapDetailCreateCommand request, CancellationToken cancellationToken)
{
    var client = new MongoClient(configuration.GetValue<string>("ConnectionStrings:MongoDbConnection"));

    try
    {
        var roadmap = await dbContext.Roadmap
            .Find(r => r.Id == request.RoadMapSectionCreateCommand.RoadmapId)
            .FirstOrDefaultAsync(cancellationToken);

        if (roadmap != null)
        {
            await dbContext.Roadmap.UpdateOneAsync(
                r => r.Id == roadmap.Id,
                Builders<Roadmap>.Update.Set(r => r.ContentJson, request.RoadMapSectionCreateCommand.ContentJson),
                cancellationToken: cancellationToken);

            foreach (var content in request.RoadMapSectionCreateCommand.Nodes)
            {
                content.RoadmapId = roadmap.Id;
                content.Id = ObjectId.GenerateNewId().ToString();
                content.CreatedAt = DateTime.UtcNow;
                content.UpdatedAt = DateTime.UtcNow;
                content.DeletedAt = null;
            }

            foreach (var content in request.RoadMapSectionCreateCommand.Edges)
            {
                content.RoadmapId = roadmap.Id;
                content.Id = ObjectId.GenerateNewId().ToString();
                content.CreatedAt = DateTime.UtcNow;
                content.UpdatedAt = DateTime.UtcNow;
                content.DeletedAt = null;
            }

            await dbContext.Node.InsertManyAsync(request.RoadMapSectionCreateCommand.Nodes, cancellationToken: cancellationToken);
            await dbContext.Edge.InsertManyAsync(request.RoadMapSectionCreateCommand.Edges, cancellationToken: cancellationToken);

            logger.LogInformation("Roadmap Detail created successfully with id: {RoadmapDetailId}", roadmap.Id);
            return new ResponseModel(HttpStatusCode.OK, "Roadmap Detail created");
        }

        return new ResponseModel(HttpStatusCode.BadRequest, "Roadmap not found");
    }
    catch (Exception e)
    {
        logger.LogError("Error writing to MongoDB: " + e.Message);
        return new ResponseModel(HttpStatusCode.BadRequest, "Unable to create roadmap section");
    }
}
}
