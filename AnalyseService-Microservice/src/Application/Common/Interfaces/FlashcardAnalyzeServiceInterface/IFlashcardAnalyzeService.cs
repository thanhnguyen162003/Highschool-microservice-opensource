using Application.Common.Models.FlashcardModel;
using Domain.Entities;

namespace Application.Common.Interfaces.FlashcardAnalyzeServiceInterface;

public interface IFlashcardAnalyzeService
{
    public double? CalculateDifficultyScore(FlashcardAnalyticRecord record);
    public double? CalculateOptimizationScore(FlashcardAnalyticRecord record);
    public TimeSpan? CalculateTimeToMastery(FlashcardAnalyticRecord record);
    public string GetLearningSegment(UserLearningPatternRecord pattern);
    public double CalculateAverageOptimizationScore(List<FlashcardAnalyticDto> analytics);
    public double CalculateRetentionRate(List<FlashcardAnalyticRecord> analytics);
    public TrendAnalysisDto CalculateTrends(List<SessionAnalyticDto> sessions,
        List<FlashcardAnalyticRecord> flashcards);
    public RecommendationsDto GenerateRecommendations(UserFlashcardAnalyticsResponse data);
    
}
