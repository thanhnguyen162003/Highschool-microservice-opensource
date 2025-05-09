namespace Application.Common.Models.FlashcardModel;

public class FlashcardAnalyticDto
{
    public Guid FlashcardId { get; set; }
    public int ViewCount { get; set; }
    public int FlipCount { get; set; }
    public long TotalTimeSpentMs { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public double AccuracyRate { get; set; }
    public double AverageAnswerTimeMs { get; set; }
    public DateTime? LastViewDate { get; set; }
    public DateTime? NextScheduledReview { get; set; }
    public int? RepetitionNumber { get; set; }
    public double? EaseFactor { get; set; }
    public int? IntervalDays { get; set; }
    public double LearningEfficiencyScore { get; set; }
    public double ForgettingIndex { get; set; }
    public double ReviewPriority { get; set; }
    public DateTime? PredictedForgettingDate { get; set; }
    public Dictionary<string, int> StudyContexts { get; set; } = new();
    public Dictionary<string, int> DailyViewCounts { get; set; } = new();
    public double? DifficultyScore { get; set; }
    public double? OptimizationScore { get; set; }
    public TimeSpan? TimeToMastery { get; set; }
    public int DaysSinceLastReview => LastViewDate.HasValue ? (int)(DateTime.UtcNow - LastViewDate.Value).TotalDays : 0;
    public string ReviewStatus => GetReviewStatus();
    
    // User-friendly formatted fields
    public string? FormattedAccuracyRate { get; set; }
    public string? FormattedTimeSpent { get; set; }
    public string? FormattedAverageAnswerTime { get; set; }
    public string? FormattedEfficiencyScore { get; set; }
    public string? FormattedForgettingIndex { get; set; }
    public string? FormattedReviewPriority { get; set; }
    public string? FormattedTimeToMastery { get; set; }

    private string GetReviewStatus()
    {
        if (!NextScheduledReview.HasValue)
            return "Not Scheduled";
            
        if (NextScheduledReview.Value < DateTime.UtcNow)
            return "Overdue";
                
        if (NextScheduledReview.Value < DateTime.UtcNow.AddDays(1))
            return "Due Today";
                
        if (NextScheduledReview.Value < DateTime.UtcNow.AddDays(3))
            return "Due Soon";
                
        return "Scheduled";
    }
}
