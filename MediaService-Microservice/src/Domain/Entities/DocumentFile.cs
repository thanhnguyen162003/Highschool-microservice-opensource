using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;
public class DocumentFile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid DocumentId { get; set; }
    public string DocumentFileUrl { get; set; }
    public string DocumentFileType { get; set; }
    public string DocumentFileExtension { get; set; }
    public string? DocumentImagePreview { get; set; }
}
