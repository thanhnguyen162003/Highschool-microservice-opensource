using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class UserImageAvatar : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string AvatarImage { get; set; }
}
