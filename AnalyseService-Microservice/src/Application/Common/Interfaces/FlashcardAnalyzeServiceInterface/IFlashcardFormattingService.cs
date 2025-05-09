using Application.Common.Models.FlashcardModel;

namespace Application.Common.Interfaces.FlashcardAnalyzeServiceInterface;

    /// <summary>
    /// Service interface for formatting flashcard analytics data in user-friendly formats
    /// </summary>
    public interface IFlashcardFormattingService
    {
        /// <summary>
        /// Formats a raw accuracy rate (e.g. 66.66666666666666) into a clean percentage (e.g. "67%")
        /// </summary>
        string FormatAccuracy(double accuracyRate);

        /// <summary>
        /// Formats time in milliseconds to a human-readable format
        /// </summary>
        string FormatTimeSpent(double milliseconds);

        /// <summary>
        /// Formats the learning efficiency score into a user-friendly format with a descriptive label
        /// </summary>
        string FormatEfficiencyScore(double score);

        /// <summary>
        /// Formats the forgetting index into a user-friendly format with descriptive text
        /// </summary>
        string FormatForgettingIndex(double index);

        /// <summary>
        /// Formats the study frequency to a more intuitive representation
        /// </summary>
        string FormatStudyFrequency(double frequency);

        /// <summary>
        /// Formats the review priority into an easy-to-understand urgency level
        /// </summary>
        string FormatReviewPriority(double priority);

        /// <summary>
        /// Formats time to mastery into a user-friendly timespan description
        /// </summary>
        string FormatTimeToMastery(TimeSpan? timeSpan);

        /// <summary>
        /// Applies all formatting to a FlashcardAnalyticDto object
        /// </summary>
        void FormatFlashcardAnalytics(FlashcardAnalyticDto analytics);

        /// <summary>
        /// Applies formatting to an AnalyticsSummaryDto object
        /// </summary>
        void FormatAnalyticsSummary(AnalyticsSummaryDto summary);

        /// <summary>
        /// Applies formatting to a UserLearningPatternDto object
        /// </summary>
        void FormatLearningPattern(UserLearningPatternDto pattern);

        /// <summary>
        /// Format the full user analytics response with all user-friendly formats
        /// </summary>
        void FormatUserAnalytics(UserFlashcardAnalyticsResponse response);
    }
