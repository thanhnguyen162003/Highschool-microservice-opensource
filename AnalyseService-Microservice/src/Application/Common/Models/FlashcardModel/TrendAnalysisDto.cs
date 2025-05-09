namespace Application.Common.Models.FlashcardModel;

public class TrendAnalysisDto
{
    public Dictionary<DateTime, double> AccuracyTrend { get; set; } = new();
    public Dictionary<DateTime, double> EfficiencyTrend { get; set; } = new();
    public Dictionary<DateTime, long> StudyTimeTrend { get; set; } = new();
    public bool IsImproving { get; set; }
    public string MostProductiveTime { get; set; }
    public double WeeklyGrowthRate { get; set; }
}
