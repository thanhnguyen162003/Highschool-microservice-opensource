using Application.Common.Algorithms;

namespace Application.Common.Models.FlashcardFeatureModel;

/// <summary>
/// Model chứa thông tin chi tiết về tiến độ học tập của người dùng theo thuật toán FSRS 5
/// </summary>
public class FSRSDetailProgressModel
{
    public Guid FlashcardId { get; set; }
    public string FlashcardName { get; set; } = string.Empty;
    public int TotalTerms { get; set; }
    public int MasteredTerms { get; set; }
    public int LearningTerms { get; set; }
    public int ReviewTerms { get; set; }
    public int RelearningTerms { get; set; }
    public int NewTerms { get; set; }
    public double AverageDifficulty { get; set; }
    public double AverageStability { get; set; }
    public int TermsDueToday { get; set; }
    public int TermsDueTomorrow { get; set; }
    public int TermsDueThisWeek { get; set; }
    
    public List<CardDetail> CardDetails { get; set; } = new List<CardDetail>();
    
    /// <summary>
    /// Chi tiết về một card cụ thể
    /// </summary>
    public class CardDetail
    {
        public Guid ContentId { get; set; }
        public string Term { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public double Difficulty { get; set; }
        public double Stability { get; set; }
        public string State { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public double TimeSpent { get; set; }
        public double Retrievability { get; set; }
        
        // Trong FSRS 5, một card được coi là thành thạo khi ở trạng thái Review và có độ ổn định Stability > 30
        public bool IsMastered
        {
            get => State == FSRSAlgorithm.State.Review.ToString() && Stability > 30;
            set { /* Setter giả để duy trì khả năng tương thích API */ }
        }
        
        // Thời gian đến lần ôn tập tiếp theo
        public string TimeToNextReview
        {
            get
            {
                if (!DueDate.HasValue)
                    return "Chưa học";
                    
                var timeSpan = DueDate.Value - DateTime.UtcNow;
                
                if (timeSpan.TotalDays < 0)
                    return "Đến hạn";
                else if (timeSpan.TotalDays < 1)
                    return $"{(int)timeSpan.TotalHours} giờ";
                else if (timeSpan.TotalDays < 30)
                    return $"{(int)timeSpan.TotalDays} ngày";
                else
                    return $"{(int)(timeSpan.TotalDays / 30)} tháng";
            }
        }
    }
} 