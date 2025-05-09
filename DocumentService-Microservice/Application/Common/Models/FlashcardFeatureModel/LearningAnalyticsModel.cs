namespace Application.Common.Models.FlashcardFeatureModel;

/// <summary>
/// Model chứa thông tin phân tích hiệu suất học tập của người dùng
/// </summary>
public class LearningAnalyticsModel
{
    // Tổng quan
    public int TotalFlashcardSets { get; set; }
    public int TotalCards { get; set; }
    public int TotalMasteredCards { get; set; }
    public double OverallMasteryPercentage { get; set; }
    public double AverageRetentionRate { get; set; }
    
    // Số liệu theo ngày
    public List<DailyActivity> DailyActivities { get; set; } = new List<DailyActivity>();
    
    // Phân phối độ khó
    public Dictionary<string, int> DifficultyDistribution { get; set; } = new Dictionary<string, int>
    {
        { "Rất dễ (1-3)", 0 },
        { "Dễ (3-5)", 0 },
        { "Trung bình (5-7)", 0 },
        { "Khó (7-9)", 0 },
        { "Rất khó (9-10)", 0 }
    };
    
    // Phân phối trạng thái học
    public Dictionary<string, int> StateDistribution { get; set; } = new Dictionary<string, int>
    {
        { "Mới", 0 },
        { "Đang học", 0 },
        { "Ôn tập", 0 },
        { "Học lại", 0 },
        { "Đã thuộc", 0 }
    };
    
    // Số thẻ sắp đến hạn
    public int CardsDueToday { get; set; }
    public int CardsDueTomorrow { get; set; }
    public int CardsDueThisWeek { get; set; }
    
    // Ước tính thời gian học tập
    public TimeSpan EstimatedStudyTimeToday { get; set; }
    public TimeSpan EstimatedStudyTimeThisWeek { get; set; }
    
    // Flashcard sets đang học
    public List<FlashcardSetProgress> ActiveFlashcardSets { get; set; } = new List<FlashcardSetProgress>();

    /// <summary>
    /// Mô tả hoạt động học tập trong một ngày
    /// </summary>
    public class DailyActivity
    {
        public DateTime Date { get; set; }
        public int CardsStudied { get; set; }
        public int NewCards { get; set; }
        public int ReviewedCards { get; set; }
        public int CorrectResponses { get; set; }
        public int IncorrectResponses { get; set; }
        public double RetentionRate { get; set; }
        public TimeSpan StudyTime { get; set; }
    }

    /// <summary>
    /// Mô tả tiến độ học một bộ flashcard
    /// </summary>
    public class FlashcardSetProgress
    {
        public Guid FlashcardId { get; set; }
        public string FlashcardName { get; set; } = string.Empty;
        public int TotalCards { get; set; }
        public int MasteredCards { get; set; }
        public double ProgressPercentage { get; set; }
        public DateTime LastStudied { get; set; }
        public int CardsDueToday { get; set; }
    }
} 