using Domain.Common;

namespace Domain.Entities;

public class Certificate : BaseAuditableEntity
{
	public Guid Id { get; set; }

	public Guid TeacherId { get; set; }

	public string CertName { get; set; } = null!;

	public string? CertLink { get; set; }

	public string? IssuedBy { get; set; }

	public DateTime? DeletedAt { get; set; }

	public DateTime? IssueDate { get; set; }

	public virtual Teacher Teacher { get; set; } = null!;
}
