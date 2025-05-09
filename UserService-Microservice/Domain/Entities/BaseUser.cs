using Domain.Common;

namespace Domain.Entities;

public class BaseUser : BaseAuditableEntity
{
	public Guid Id { get; set; }

	public string? Username { get; set; }

	public string? Email { get; set; } = null!;

	public string? Bio { get; set; }

	public string? Fullname { get; set; }

	public byte[]? PasswordSalt { get; set; } = null!;

	public byte[]? Password { get; set; } = null!;

	public int? RoleId { get; set; }

	public string? Provider { get; set; }

	public string? Status { get; set; }

	public string? Timezone { get; set; }

	public DateTime? LastLoginAt { get; set; }

	public string? ProfilePicture { get; set; }

	public DateTime? DeletedAt { get; set; }

	public string? Address { get; set; }

	public string? ProgressStage { get; set; }

	public DateTime? Birthdate { get; set; }

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

	public virtual ICollection<RecentView> RecentViews { get; set; } = new List<RecentView>();

	public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

	public virtual ICollection<ReportDocument> ReportDocuments { get; set; } = new List<ReportDocument>();

	public virtual Role? Role { get; set; }

	public virtual Student? Student { get; set; }

	public virtual Teacher? Teacher { get; set; }

	public virtual ICollection<UserSubject> UserSubjects { get; set; } = new List<UserSubject>();

	public virtual Roadmap? Roadmap { get; set; }
}