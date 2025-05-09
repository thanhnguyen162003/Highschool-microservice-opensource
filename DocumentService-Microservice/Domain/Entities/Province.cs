using Domain.Common;

namespace Domain.Entities;

public class Province : BaseAuditableEntity /* Department of Education and Training */
{
    public int Id { get; set; }
    public string? ProvinceName { get; set; }
    public ICollection<School> Schools { get; set; } = new List<School>();
    public DateTime? DeletedAt { get; set; }
}