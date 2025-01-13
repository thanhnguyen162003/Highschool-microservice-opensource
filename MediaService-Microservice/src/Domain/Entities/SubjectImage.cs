using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class SubjectImage : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid SubjectId { get; set; }

    public string PublicIdUrl { get; set; }

    public string Format { get; set; }
    
    public string SubjectImageUrl { get; set; }
}
