using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
	public class UserFlashcardProgress
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid FlashcardContentId { get; set; }
		
		// Navigation property
		public FlashcardContent FlashcardContent { get; set; } = null!;
		
		// FSRS 5 fields
		public int Rating { get; set; }
		public double Difficulty { get; set; } = 5.0;
		public double Stability { get; set; } = 0.0;
		public string State { get; set; } = "New"; // New, Learning, Review, Relearning
		public DateTime? DueDate { get; set; } // When the card is due for review
		public DateTime? LastReviewDate { get; set; } // Date of the last review

        // Lịch sử thay đổi của LastReviewDate
        [Column(TypeName = "timestamp with time zone[]")]
        public List<DateTime> LastReviewDateHistory { get; set; } = new List<DateTime>();
		
		// Tổng số lần luyện tập, bao gồm mọi trạng thái (New, Learning, Review, Relearning)
		public double TimeSpent { get; set; } = 0;

        // Lịch sử thay đổi của TimeSpent
        [Column(TypeName = "double precision[]")]
        public List<double> TimeSpentHistory { get; set; } = new List<double>();
	}
}