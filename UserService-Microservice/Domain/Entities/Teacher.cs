namespace Domain.Entities;

public class Teacher
{
	public Guid Id { get; set; }

	public Guid BaseUserId { get; set; }

	public string? GraduatedUniversity { get; set; }

	public string? ContactNumber { get; set; }

	public string? Pin { get; set; }

	public string? WorkPlace { get; set; }

	public string? SubjectsTaught { get; set; }

	public double? Rating { get; set; }

	public int ExperienceYears { get; set; }

	public bool Verified { get; set; }

	public string? VideoIntroduction { get; set; }

	public virtual BaseUser BaseUser { get; set; } = null!;

	public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}