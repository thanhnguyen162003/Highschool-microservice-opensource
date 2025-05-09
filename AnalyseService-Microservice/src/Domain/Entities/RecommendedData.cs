using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class RecommendedData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public List<Guid>? SubjectIds { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid? UserId { get; set; }
    public int Grade { get; set; }
    public string? TypeExam { get; set; }
}
