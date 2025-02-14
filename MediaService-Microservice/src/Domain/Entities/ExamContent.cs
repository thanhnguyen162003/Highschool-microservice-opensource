using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class ExamContent : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public Guid Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid ExamId { get; set; }

    public string ExamFile { get; set; }

    public string FileType { get; set; }
}
