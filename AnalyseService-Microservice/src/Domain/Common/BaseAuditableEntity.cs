using System.Text.Json.Serialization;

namespace Discussion_Microservice.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public DateTime? DeletedAt { get; set; }
}
