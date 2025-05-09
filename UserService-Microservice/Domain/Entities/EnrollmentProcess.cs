using Domain.Common;

namespace Domain.Entities;

public class EnrollmentProcess : BaseAuditableEntity
{
	public Guid Id { get; set; }

	public Guid EnrollmentId { get; set; }

	public int ChapterId { get; set; }

	public int LessonId { get; set; }

	public DateTime? DeletedAt { get; set; }

	public virtual Enrollment Enrollment { get; set; } = null!;
}
