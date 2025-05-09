using Domain.Common;

namespace Domain.Entities;

public class Note : BaseAuditableEntity
{
	public Guid Id { get; set; }

	public string NoteName { get; set; } = null!;

	public string? NoteBody { get; set; }

	public Guid UserId { get; set; }
	public DateTime? DeletedAt { get; set; }

	public virtual BaseUser User { get; set; } = null!;
}
