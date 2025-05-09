namespace Application.Features.AnalyseFeature.Supports;

public class SessionAnalytics
{
    public Guid UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public long TotalTimeSpentMs { get; set; }
    public int CardsStudied { get; set; }
    public int CorrectAnswers { get; set; }
    public double AccuracyRate { get; set; }
    public double AverageTimePerCardMs { get; set; }
    public string? StudyMode { get; set; }
    public string? StudyContext { get; set; }
    public List<Guid> FlashcardIds { get; set; } = new List<Guid>();
    public TimeSpan? TimeOfDay { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
}
