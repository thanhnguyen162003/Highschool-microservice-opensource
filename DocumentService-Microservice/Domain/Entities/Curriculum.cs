using Domain.Common;

namespace Domain.Entities;

public class Curriculum : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public string? CurriculumName { get; set; }
	public string? ImageUrl { get; set; }
	public bool IsExternal { get; set; }
	public DateTime? DeletedAt { get; set; }
    public virtual ICollection<SubjectCurriculum> SubjectCurricula { get; set; } = new List<SubjectCurriculum>();
} 