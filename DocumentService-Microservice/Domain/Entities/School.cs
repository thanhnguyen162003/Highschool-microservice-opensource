using Domain.Common;

namespace Domain.Entities;

public class School : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public string? SchoolName { get; set; }
    public int? ProvinceId { get; set; }
    public virtual Province Province { get; set; } = null!;
    public string? LocationDetail { get; set; }
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public DateTime? DeletedAt { get; set; }
}