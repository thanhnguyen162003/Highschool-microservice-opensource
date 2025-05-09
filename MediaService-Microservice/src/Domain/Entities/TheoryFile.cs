using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class TheoryFile : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid TheoryId { get; set; }
    
    public string File { get; set; }
    
    public string FileType { get; set; }
    
    public string FileExtention { get; set; }
    
    public string PublicId { get; set; }
}
