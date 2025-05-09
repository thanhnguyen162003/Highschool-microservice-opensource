namespace Application.Common.Models.FlashcardFeatureModel;

/// <summary>
/// Model chứa thông tin về các flashcard đến hạn cần ôn tập theo thuật toán FSRS
/// </summary>
public class DueFlashcardModel
{
    public Guid FlashcardId { get; set; }
    public string FlashcardName { get; set; } = string.Empty;
    public string? FlashcardDescription { get; set; }
    public int DueCardCount { get; set; }
    public int TotalCardCount { get; set; }
    public double ProgressPercentage { get; set; }
    public bool IsReview { get; set; } // Đánh dấu đây là phiên ôn tập hay học mới
    public List<DueCard> DueCards { get; set; } = new List<DueCard>();

    /// <summary>
    /// Thông tin về một card đến hạn cần ôn tập
    /// </summary>
    public class DueCard
    {
        public Guid ContentId { get; set; }
        public string Term { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public string? TermRichText { get; set; } = string.Empty;
        public string? DefinitionRichText { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        //public string State { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        //public double Difficulty { get; set; }
        //public double Stability { get; set; }
        //public double Retrievability { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsNew { get; set; } // Đánh dấu là thẻ mới chưa học
        public bool IsReview { get; set; } // Đánh dấu là phiên ôn tập thẻ đã học
        public bool IsDueToday { get; set; } // Đánh dấu là thẻ đến hạn ngày hôm nay
        //public bool IsEarlyReview { get; set; } // Đánh dấu là ôn tập sớm (trước hạn)
        //public bool IsLowRating { get; set; } // Đánh dấu là thẻ có rating thấp (1 hoặc 2)
        //public int Rating { get; set; } // Rating của thẻ (1-4)
        
        //// Độ ưu tiên: càng thấp càng ưu tiên cao (đến học trước)
        public int Priority { get; set; }
    }
} 