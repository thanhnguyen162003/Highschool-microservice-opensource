
namespace Domain.Entities
{
	public class UserFlashcardProgress
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid FlashcardContentId { get; set; }
		public Guid FlashcardId { get; set; }
		public int CorrectCount { get; set; } = 0;
		public DateTime? LastStudiedAt { get; set; }
		public bool IsMastered { get; set; } = false;
		// New SM2-specific fields
		public double EaseFactor { get; set; } = 2.5; // Default initial E-Factor
		public int Interval { get; set; } = 1; // Initial interval is 1 session
		public int RepetitionCount { get; set; } = 0; // Number of consecutive correct answers
	}
}
