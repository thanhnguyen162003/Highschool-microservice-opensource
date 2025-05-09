namespace Application.Common.Models.StatisticModel;

public class OwnedStatistic
{
    public int CurrentLoginStreak { get; set; }
    public int LongestLoginStreak { get; set; }
    public int CurrentLearnStreak { get; set; }
    public int LongestLearnStreak { get; set; }
    public int TodayLessonLearned { get; set; }
    public int TotalLessonLearned { get; set; }
    public int TotalFlashcardLearned { get; set; }
    public int TotalFlashcardContentLearned { get; set; }
    public int TotalFlashcardLearnDates { get; set; }
    public double TotalFlashcardContentHours { get; set; }
}
