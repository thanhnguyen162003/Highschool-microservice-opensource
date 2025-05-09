namespace Application.Common.Models.FlashcardModel;

public class UserLearningPatternDto
{
    public double StudyFrequency { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastStudyDate { get; set; }
    public int? OptimalStudyHour { get; set; }
    public int? MostFrequentStudyDay { get; set; }
    public string? PrimaryStudyContext { get; set; }
    public Dictionary<int, int> StudyHours { get; set; } = new();
    public Dictionary<int, int> EffectiveStudyHours { get; set; } = new();
    public Dictionary<int, int> StudyDaysOfWeek { get; set; } = new();
    public Dictionary<string, int> StudyContexts { get; set; } = new();
    public Dictionary<string, bool> StudyDaysInYear { get; set; } = new();
    public string? LearningSegment { get; set; }
    public double AverageOptimizationScore { get; set; }
    public string RecommendedStudyTime => GetRecommendedStudyTime();
    
    // User-friendly formatted fields
    public string? FormattedStudyFrequency { get; set; }
    public string? FormattedOptimizationScore { get; set; }
        
    private string GetRecommendedStudyTime()
    {
        if (!OptimalStudyHour.HasValue || !MostFrequentStudyDay.HasValue)
            return "Not enough data";
                
        string dayName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)MostFrequentStudyDay.Value);
        return $"{dayName}s at {OptimalStudyHour.Value}:00";
    }
}
