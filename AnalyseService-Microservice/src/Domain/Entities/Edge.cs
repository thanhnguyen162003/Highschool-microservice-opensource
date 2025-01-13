using System.Text.Json.Serialization;
using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Edge : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string RoadmapId { get; set; }

    public string? Source { get; set; }

    public string? Target { get; set; }

    public string? SourceHandle { get; set; }
    
    public string? TargetHandle { get; set; }
    
    public string? Type { get; set; }

    public string EdgeId { get; set; }
    
    public bool? Animated { get; set; }
}
