using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class DiscussionAttachment : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public Guid Id { get; set; }

    public Guid DiscussionId { get; set; }
    public string DiscussionFile { get; set; }
    public string FileType { get; set; }
    public string PublicIdUrl { get; set; }
    public string Format { get; set; }
}
