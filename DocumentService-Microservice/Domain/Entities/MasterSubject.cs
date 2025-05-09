using Domain.Common;

namespace Domain.Entities;

public class MasterSubject : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public string MasterSubjectName { get; set; } = null!;
    public string MasterSubjectSlug { get; set; } = null!;
    public virtual required ICollection<Subject> Subjects { get; set; }
}