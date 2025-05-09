using Discussion_Microservice.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class UserImageCertificate : BaseAuditableEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid CertificateId { get; set; }
    
    public string CertImage { get; set; }
}
