namespace Application.Common.Models.FlashcardModel;

public class SessionAnalyticDto
{
    public Guid SessionId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public long TotalTimeSpentMs { get; set; }
    public int CardsStudied { get; set; }
    public int CorrectAnswers { get; set; }
    public double AccuracyRate { get; set; }
    public double AverageTimePerCardMs { get; set; }
    public string StudyMode { get; set; }
    public string StudyContext { get; set; }
    public List<Guid> FlashcardIds { get; set; } = new();
    public TimeSpan? TimeOfDay { get; set; }
    public int? DayOfWeek { get; set; }
    public string FormattedStudyTime => 
        $"{(TotalTimeSpentMs / 1000 / 60):0} minutes {(TotalTimeSpentMs / 1000 % 60):0} seconds";
    public string FormattedAccuracy => $"{AccuracyRate:0.0}%";
}
