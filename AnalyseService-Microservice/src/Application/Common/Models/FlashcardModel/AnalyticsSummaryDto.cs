namespace Application.Common.Models.FlashcardModel;

public class AnalyticsSummaryDto
{
    public int TotalFlashcardsStudied { get; set; }
    public int TotalStudySessions { get; set; }
    public long TotalTimeSpentMs { get; set; }
    public double AverageAccuracy { get; set; }
    public double AverageEfficiencyScore { get; set; }
    public List<Guid> DifficultFlashcards { get; set; } = new();
    public List<Guid> MasteredFlashcards { get; set; } = new();
    public List<Guid> DueForReviewFlashcards { get; set; } = new();
    public string FormattedTotalStudyTime => 
        $"{(TotalTimeSpentMs / 1000 / 60 / 60):0} hours {(TotalTimeSpentMs / 1000 / 60 % 60):0} minutes";
    public int OverdueFlashcards { get; set; }
    public int DueFlashcardsToday { get; set; }
    public int MasteryLevel => CalculateMasteryLevel();
    public double RetentionRate { get; set; }
    public double AverageTimePerCardMs { get; set; }
    
    // User-friendly formatted fields
    public string? FormattedTotalTimeSpent { get; set; }
    public string? FormattedAverageAccuracy { get; set; }
    public string? FormattedAverageEfficiency { get; set; }
    public string? FormattedRetentionRate { get; set; }
    public string? FormattedAverageTimePerCard { get; set; }
        
    private int CalculateMasteryLevel()
    {
        if (TotalFlashcardsStudied == 0) return 0;
            
        double masteryRatio = (double)MasteredFlashcards.Count / TotalFlashcardsStudied;
        if (masteryRatio > 0.8) return 5;
        if (masteryRatio > 0.6) return 4;
        if (masteryRatio > 0.4) return 3;
        if (masteryRatio > 0.2) return 2;
        return 1;
    }
}
