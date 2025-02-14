using System.Text.Json.Serialization;
using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Node : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string RoadmapId { get; set; }
    
    public int? PositionX { get; set; }
    
    public int? PositionY { get; set; }
    
    public string? Type { get; set; }
    
    public string? DataLabel { get; set; }
    
    public string? NodeId { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid? DataId { get; set; }
}
