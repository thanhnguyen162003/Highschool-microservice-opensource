namespace Application.Common.Models.FlashcardModel;

public class UserFlashcardAnalyticsResponse
{
    public Guid UserId { get; set; }
    public List<FlashcardAnalyticDto> FlashcardAnalytics { get; set; } = new();
    public UserLearningPatternDto LearningPattern { get; set; }
    public List<SessionAnalyticDto> RecentSessions { get; set; } = new();
    public AnalyticsSummaryDto Summary { get; set; } = new();
    public TrendAnalysisDto Trends { get; set; } = new();
    public RecommendationsDto Recommendations { get; set; } = new();
}
