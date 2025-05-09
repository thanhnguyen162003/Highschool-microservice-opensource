namespace Application.Features.AnalyseFeature.Supports;

// Class lưu trữ mẫu học tập của người dùng (không phải entity)
public class UserLearningPatterns
{
    public double StudyFrequency { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastStudyDate { get; set; }
    public int? OptimalStudyHour { get; set; }
    public int? MostFrequentStudyDay { get; set; }
    public string? PrimaryStudyContext { get; set; }
    
    // Lưu trữ số liệu theo thời gian
    public Dictionary<int, int>? StudyHours { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int>? EffectiveStudyHours { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int>? StudyDaysOfWeek { get; set; } = new Dictionary<int, int>();
    public Dictionary<string, int>? StudyContexts { get; set; } = new Dictionary<string, int>();
    public Dictionary<int, int>? StudyDaysInYear { get; set; } = new Dictionary<int, int>();
}

