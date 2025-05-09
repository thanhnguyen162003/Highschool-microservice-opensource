using Application.Common.Interfaces.FlashcardAnalyzeServiceInterface;
using Application.Common.Models.FlashcardModel;

namespace Application.Services.FlashcardAnalyze;

public class FlashcardFormattingService : IFlashcardFormattingService
{
    /// <summary>
    /// Formats a raw accuracy rate (e.g. 66.66666666666666) into a clean percentage (e.g. "67%")
    /// </summary>
    public string FormatAccuracy(double accuracyRate)
    {
        return $"{Math.Round(accuracyRate)}%";
    }

    /// <summary>
    /// Formats time in milliseconds to a human-readable format
    /// </summary>
    public string FormatTimeSpent(double milliseconds)
    {
        if (milliseconds < 1000)
            return $"{milliseconds}ms";
        
        if (milliseconds < 60000)
            return $"{Math.Round(milliseconds / 1000.0, 1)}s";
        
        var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
        if (timeSpan.TotalHours < 1)
            return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
        
        if (timeSpan.TotalDays < 1)
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
        
        return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h";
    }

    /// <summary>
    /// Formats the learning efficiency score into a user-friendly format with a descriptive label
    /// </summary>
    public string FormatEfficiencyScore(double score)
    {
        var roundedScore = Math.Round(score, 1);
        var category = GetEfficiencyCategory(score);
        return $"{roundedScore} ({category})";
    }

    /// <summary>
    /// Formats the forgetting index into a user-friendly format with descriptive text
    /// </summary>
    public string FormatForgettingIndex(double index)
    {
        // Convert to a 0-100 scale that's easier to understand
        var adjustedIndex = Math.Min(Math.Max(index, -10), 10);
        var normalizedIndex = (adjustedIndex + 10) * 5; // Convert from -10/10 scale to 0-100
        var roundedIndex = Math.Round(normalizedIndex);
        
        string risk;
        if (roundedIndex < 30) risk = "Low";
        else if (roundedIndex < 60) risk = "Medium";
        else risk = "High";
        
        return $"{roundedIndex}% ({risk} forgetting risk)";
    }

    /// <summary>
    /// Formats the study frequency to a more intuitive representation
    /// </summary>
    public string FormatStudyFrequency(double frequency)
    {
        // Convert from 0-1 scale to days per week
        var daysPerWeek = Math.Round(frequency * 7, 1);
        return $"{daysPerWeek} days/week";
    }

    /// <summary>
    /// Formats the review priority into an easy-to-understand urgency level
    /// </summary>
    public string FormatReviewPriority(double priority)
    {
        var urgency = GetPriorityUrgency(priority);
        var roundedPriority = Math.Round(priority, 1);
        return $"{roundedPriority}/10 ({urgency})";
    }

    /// <summary>
    /// Formats time to mastery into a user-friendly timespan description
    /// </summary>
    public string FormatTimeToMastery(TimeSpan? timeSpan)
    {
        if (!timeSpan.HasValue)
            return "Not mastered yet";
            
        return FormatTimeSpanToWords(timeSpan.Value);
    }

    /// <summary>
    /// Applies all formatting to a FlashcardAnalyticDto object
    /// </summary>
    public void FormatFlashcardAnalytics(FlashcardAnalyticDto analytics)
    {
        // Add formatted versions of the complex numerical values
        analytics.FormattedAccuracyRate = FormatAccuracy(analytics.AccuracyRate);
        analytics.FormattedTimeSpent = FormatTimeSpent(analytics.TotalTimeSpentMs);
        analytics.FormattedAverageAnswerTime = FormatTimeSpent(analytics.AverageAnswerTimeMs);
        analytics.FormattedEfficiencyScore = FormatEfficiencyScore(analytics.LearningEfficiencyScore);
        analytics.FormattedForgettingIndex = FormatForgettingIndex(analytics.ForgettingIndex);
        analytics.FormattedReviewPriority = FormatReviewPriority(analytics.ReviewPriority);
        
        if (analytics.TimeToMastery.HasValue)
        {
            analytics.FormattedTimeToMastery = FormatTimeToMastery(analytics.TimeToMastery);
        }
    }

    /// <summary>
    /// Applies formatting to an AnalyticsSummaryDto object
    /// </summary>
    public void FormatAnalyticsSummary(AnalyticsSummaryDto summary)
    {
        summary.FormattedTotalTimeSpent = FormatTimeSpent(summary.TotalTimeSpentMs);
        summary.FormattedAverageAccuracy = FormatAccuracy(summary.AverageAccuracy);
        summary.FormattedAverageEfficiency = FormatEfficiencyScore(summary.AverageEfficiencyScore);
        summary.FormattedRetentionRate = FormatAccuracy(summary.RetentionRate);
        summary.FormattedAverageTimePerCard = FormatTimeSpent(summary.AverageTimePerCardMs);
    }

    /// <summary>
    /// Applies formatting to a UserLearningPatternDto object
    /// </summary>
    public void FormatLearningPattern(UserLearningPatternDto pattern)
    {
        pattern.FormattedStudyFrequency = FormatStudyFrequency(pattern.StudyFrequency);
        
        if (pattern.AverageOptimizationScore > 0)
        {
            pattern.FormattedOptimizationScore = FormatEfficiencyScore(pattern.AverageOptimizationScore);
        }
    }

    /// <summary>
    /// Format the full user analytics response with all user-friendly formats
    /// </summary>
    public void FormatUserAnalytics(UserFlashcardAnalyticsResponse response)
    {
        // Format all flashcard analytics
        foreach (var analytics in response.FlashcardAnalytics)
        {
            FormatFlashcardAnalytics(analytics);
        }
        
        // Format summary if available
        if (response.Summary != null)
        {
            FormatAnalyticsSummary(response.Summary);
        }
        
        // Format learning pattern if available
        if (response.LearningPattern != null)
        {
            FormatLearningPattern(response.LearningPattern);
        }
    }

    #region Helper methods
    private string GetEfficiencyCategory(double score)
    {
        if (score < 10) return "Needs improvement";
        if (score < 30) return "Basic";
        if (score < 50) return "Good";
        if (score < 70) return "Very good";
        return "Excellent";
    }
    
    private string GetPriorityUrgency(double priority)
    {
        if (priority < 3) return "Low priority";
        if (priority < 6) return "Medium priority";
        if (priority < 8) return "High priority";
        return "Urgent";
    }
    
    private string FormatTimeSpanToWords(TimeSpan span)
    {
        if (span.TotalDays > 30)
        {
            int months = (int)(span.TotalDays / 30);
            return months == 1 ? "1 month" : $"{months} months";
        }
        
        if (span.TotalDays >= 1)
        {
            return span.Days == 1 ? "1 day" : $"{span.Days} days";
        }
        
        if (span.TotalHours >= 1)
        {
            return span.Hours == 1 ? "1 hour" : $"{span.Hours} hours";
        }
        
        if (span.TotalMinutes >= 1)
        {
            return span.Minutes == 1 ? "1 minute" : $"{span.Minutes} minutes";
        }
        
        return span.Seconds == 1 ? "1 second" : $"{span.Seconds} seconds";
    }
    #endregion
}
