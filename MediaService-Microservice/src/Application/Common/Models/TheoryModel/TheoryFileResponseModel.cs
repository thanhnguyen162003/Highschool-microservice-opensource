using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.Common.Models.TheoryModel;

public class TheoryFileResponseModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid TheoryId { get; set; }
    
    public string File { get; set; }
    
    public string FileType { get; set; }
    
    public string FileExtention { get; set; }
    
    public string PublicId { get; set; }
}

