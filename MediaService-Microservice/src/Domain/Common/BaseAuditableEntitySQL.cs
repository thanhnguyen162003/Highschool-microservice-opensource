using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Discussion_Microservice.Domain.Common;

public abstract class BaseAuditableEntitySQL : BaseAuditableEntity
{
    [BsonRepresentation(BsonType.String)]
    public Guid CreatedBy { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid? UpdatedBy { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId ObjectId { get; set; }
}
