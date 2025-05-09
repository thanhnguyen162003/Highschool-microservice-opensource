using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class UserAnalyseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    public string? Address { get; set; }
    public int Grade { get; set; }
    public string? SchoolName { get; set; }
    public string? Major { get; set; }
    public string? TypeExam { get; set; }
    [BsonRepresentation(BsonType.String)]
    public List<Guid>? Subjects { get; set; }
}
