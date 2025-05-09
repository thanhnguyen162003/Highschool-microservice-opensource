namespace Domain.Entities;

public class Enrollment
{
	public Guid Id { get; set; }

	public Guid StudentId { get; set; }

	public int SubjectId { get; set; }

	public DateTime? UpdatedAt { get; set; }

	public DateTime CreatedAt { get; set; }

	public virtual ICollection<EnrollmentProcess> EnrollmentProcesses { get; set; } = new List<EnrollmentProcess>();

	public virtual Student Student { get; set; } = null!;
}
