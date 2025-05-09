
namespace Domain.Entities
{
	public class StarredTerm
	{
		public Guid UserId { get; set; }
		public Guid FlashcardContentId { get; set; }
		public Guid ContainerId { get; set; }
		public virtual Container? Container { get; set; }
		public virtual FlashcardContent? FlashcardContent { get; set; }
	}
}
