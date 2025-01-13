using System.Text.Json.Serialization;
using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class DocumentDay : BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public List<Guid> SubjectIdsOfTheDay { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public List<Guid> DocumentIdsOfTheDay { get; set; }

}
