using Application.Common.Interfaces.FlashcardAnalyzeServiceInterface;
using Application.Common.Models.FlashcardModel;
using Domain.Entities;

namespace Application.Services.FlashcardAnalyze;

public class FlashcardAnalyzeService : IFlashcardAnalyzeService
{
        public double? CalculateDifficultyScore(FlashcardAnalyticRecord record)
        {
            // Higher score means more difficult
            if (record.ViewCount < 2) 
                return null; // Not enough data
                
            double viewFactor = Math.Min(record.ViewCount / 5.0, 1.0); // More views give more reliable score
            
            // Base difficulty on accuracy rate (inverted), time spent, and forgetting index
            double accuracyComponent = (100 - record.AccuracyRate) * 0.6;
            double timeComponent = Math.Min(record.AverageAnswerTimeMs / 10000, 10) * 0.2; // Cap at 10 seconds
            double forgettingComponent = record.ForgettingIndex * 0.2;
            
            return (accuracyComponent + timeComponent + forgettingComponent) * viewFactor;
        }

        public double? CalculateOptimizationScore(FlashcardAnalyticRecord record)
        {
            // Higher score means better optimization
            if (record.ViewCount < 3) 
                return null; // Not enough data
                
            // Balance between accuracy and time efficiency
            double accuracyComponent = record.AccuracyRate * 0.7;
            double timeEfficiency = Math.Max(0, 10 - Math.Min(record.AverageAnswerTimeMs / 1000, 10)) * 0.3;
            
            return accuracyComponent + timeEfficiency;
        }

        public TimeSpan? CalculateTimeToMastery(FlashcardAnalyticRecord record)
        {
            if (record.AccuracyRate >= 90)
                return record.LastViewDate - record.LastUpdated;
                
            return null; // Not mastered yet
        }

        public string GetLearningSegment(UserLearningPatternRecord pattern)
        {
            if (pattern.CurrentStreak <= 0)
                return "Inactive";
                
            if (pattern.CurrentStreak >= pattern.LongestStreak * 0.8)
                return "Peak Performer";
                
            if (pattern.StudyFrequency > 0.7)
                return "Consistent Learner";
                
            if (pattern.StudyFrequency > 0.3)
                return "Regular Learner";
                
            return "Occasional Learner";
        }

        public double CalculateAverageOptimizationScore(List<FlashcardAnalyticDto> analytics)
        {
            var validScores = analytics
                .Where(a => a.OptimizationScore.HasValue)
                .Select(a => a.OptimizationScore.Value)
                .ToList();
                
            return validScores.Any() ? validScores.Average() : 0;
        }

        public double CalculateRetentionRate(List<FlashcardAnalyticRecord> analytics)
        {
            var cardsWithMultipleViews = analytics.Where(a => a.ViewCount > 1).ToList();
            if (!cardsWithMultipleViews.Any())
                return 0;
                
            // Retention rate based on accuracy improvement
            return cardsWithMultipleViews.Average(a => a.AccuracyRate);
        }

        public TrendAnalysisDto CalculateTrends(List<SessionAnalyticDto> sessions, List<FlashcardAnalyticRecord> flashcards)
        {
            var trends = new TrendAnalysisDto();
            
            if (sessions == null || !sessions.Any())
                return trends;
            
            // Group sessions by day
            var sessionsByDay = sessions
                .Where(s => s.StartTime.HasValue)
                .GroupBy(s => s.StartTime.Value.Date)
                .ToDictionary(g => g.Key, g => g.ToList());
                
            // Calculate accuracy trend
            trends.AccuracyTrend = sessionsByDay
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Average(s => s.AccuracyRate));
                
            // Calculate efficiency trend (time per card)
            trends.StudyTimeTrend = sessionsByDay
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Sum(s => s.TotalTimeSpentMs));
                
            // Determine if improving
            if (trends.AccuracyTrend.Count >= 2)
            {
                var orderedDates = trends.AccuracyTrend.Keys.OrderBy(d => d).ToList();
                var firstWeekAvg = trends.AccuracyTrend
                    .Where(kvp => kvp.Key <= orderedDates[orderedDates.Count / 2])
                    .Select(kvp => kvp.Value)
                    .DefaultIfEmpty(0)
                    .Average();
                    
                var lastWeekAvg = trends.AccuracyTrend
                    .Where(kvp => kvp.Key > orderedDates[orderedDates.Count / 2])
                    .Select(kvp => kvp.Value)
                    .DefaultIfEmpty(0)
                    .Average();
                    
                trends.IsImproving = lastWeekAvg > firstWeekAvg;
                trends.WeeklyGrowthRate = firstWeekAvg > 0 ? (lastWeekAvg - firstWeekAvg) / firstWeekAvg * 100 : 0;
            }
            
            // Find most productive time
            if (sessions.Any(s => s.TimeOfDay.HasValue))
            {
                var hourGroups = sessions
                    .Where(s => s.TimeOfDay.HasValue)
                    .GroupBy(s => s.TimeOfDay.Value.Hours)
                    .ToDictionary(g => g.Key, g => g.Average(s => s.AccuracyRate));
                    
                if (hourGroups.Any())
                {
                    var bestHour = hourGroups.OrderByDescending(kvp => kvp.Value).First().Key;
                    trends.MostProductiveTime = $"{bestHour:00}:00 - {(bestHour + 1) % 24:00}:00";
                }
            }
            
            return trends;
        }

        public RecommendationsDto GenerateRecommendations(UserFlashcardAnalyticsResponse data)
        {
            var recommendations = new RecommendationsDto();
            
            // Add study recommendations
            if (data.Summary.OverdueFlashcards > 0)
            {
                recommendations.StudyRecommendations.Add($"Review {data.Summary.OverdueFlashcards} overdue flashcards to maintain knowledge");
            }
            
            if (data.LearningPattern?.CurrentStreak < data.LearningPattern?.LongestStreak)
            {
                recommendations.StudyRecommendations.Add("Try to study consistently to rebuild your streak");
            }
            
            if (data.Trends?.IsImproving == true)
            {
                recommendations.StudyRecommendations.Add("Your accuracy is improving! Keep up the good work");
            }
            else if (data.Trends?.IsImproving == false)
            {
                recommendations.StudyRecommendations.Add("Your accuracy is declining. Consider reviewing your difficult cards more frequently");
            }
            
            // Priority review cards
            recommendations.PriorityReviewCards = data.FlashcardAnalytics
                .Where(f => f.ReviewPriority > 7)
                .OrderByDescending(f => f.ReviewPriority)
                .Take(5)
                .Select(f => f.FlashcardId)
                .ToList();
                
            // Optimal study schedule
            if (data.LearningPattern?.OptimalStudyHour.HasValue == true)
            {
                int hour = data.LearningPattern.OptimalStudyHour.Value;
                recommendations.OptimalStudySchedule = $"Study at {hour:00}:00 for optimal results based on your history";
            }
            
            // Improvement areas
            if (data.Summary.AverageAccuracy < 70)
            {
                recommendations.ImprovementAreas.Add("Focus on improving overall accuracy");
            }
            
            if (data.FlashcardAnalytics.Any(f => f.ForgettingIndex > 0.7))
            {
                recommendations.ImprovementAreas.Add("Some cards have high forgetting rates - try using spaced repetition more consistently");
            }
            
            return recommendations;
        }
        
}
