using Domain.Common;

namespace Domain.Entities;

public class RecentView : BaseAuditableEntity
{
	public Guid Id { get; set; }

	public int? SubjectId { get; set; }

	public long? DiscussionId { get; set; }

	public long? FlashcardId { get; set; }

	public Guid UserId { get; set; }

	public DateTime? DeletedAt { get; set; }

	public virtual BaseUser User { get; set; } = null!;
}
