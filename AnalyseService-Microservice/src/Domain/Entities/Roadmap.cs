using System.Text.Json.Serialization;
using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Roadmap : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; }
    
    public string RoadmapName { get; set; }

    public string ContentJson { get; set; }

    public string RoadmapDescription { get; set; }

    [BsonRepresentation(BsonType.String)]
    public List<Guid> RoadmapSubjectIds { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public List<Guid>? RoadmapDocumentIds { get; set; }
    
    public List<string> TypeExam { get; set; }
    
    public ICollection<Node> Nodes { get; set; } = new List<Node>();

    public ICollection<Edge> Edges { get; set; } = new List<Edge>();
}
