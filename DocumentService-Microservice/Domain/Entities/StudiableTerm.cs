
using Domain.Common;

namespace Domain.Entities
{
	public class StudiableTerm 
	{
		public Guid Id { get; set; }
		public string? Mode { get; set; }
		public required Guid UserId { get; set; }
		public Guid ContainerId { get; set; }
		public Guid? FlashcardContentId { get; set; }
		public virtual Container? Container { get; set; }
		public virtual FlashcardContent? FlashcardContent { get; set; }
	}
}
