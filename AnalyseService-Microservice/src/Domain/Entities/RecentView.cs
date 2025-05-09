using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class RecentView : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    public string? TypeDocument { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid IdDocument { get; set; }
    public string? SlugDocument { get; set; }
    public string? DocumentName { get; set; }
    public DateTime? Time { get; set; }
}
