using Application.Common.Interfaces.FlashcardAnalyzeServiceInterface;
using Application.Common.Models.FlashcardModel;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Application.Features.AnalyseFeature.Queries
{
    public record GetUserFlashcardAnalyticsQuery : IRequest<UserFlashcardAnalyticsResponse>
    {
        public Guid UserId { get; init; }
        public Guid? FlashcardId { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public int? MaxResults { get; init; }
        public string? SortBy { get; init; }
        public bool IncludeSessionDetails { get; init; } = true;
    }
    public class GetUserFlashcardAnalyticsQueryHandler(
        AnalyseDbContext dbContext,
        ILogger<GetUserFlashcardAnalyticsQueryHandler> logger,
        IFlashcardAnalyzeService  flashcardAnalyzeService,
        IFlashcardFormattingService formattingService)
        : IRequestHandler<GetUserFlashcardAnalyticsQuery, UserFlashcardAnalyticsResponse>
    {
        public async Task<UserFlashcardAnalyticsResponse> Handle(GetUserFlashcardAnalyticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = new UserFlashcardAnalyticsResponse
                {
                    UserId = request.UserId
                };

                // Build filter for flashcard analytics
                var flashcardFilter = Builders<FlashcardAnalyticRecord>.Filter.Eq(r => r.UserId, request.UserId);
                if (request.FlashcardId.HasValue)
                {
                    flashcardFilter = flashcardFilter & Builders<FlashcardAnalyticRecord>.Filter.Eq(r => r.FlashcardId, request.FlashcardId.Value);
                }
                if (request.StartDate.HasValue)
                {
                    flashcardFilter = flashcardFilter & Builders<FlashcardAnalyticRecord>.Filter.Gte(r => r.LastViewDate, request.StartDate.Value);
                }
                if (request.EndDate.HasValue)
                {
                    flashcardFilter = flashcardFilter & Builders<FlashcardAnalyticRecord>.Filter.Lte(r => r.LastViewDate, request.EndDate.Value);
                }

                // Get flashcard analytics with sorting if specified
                var flashcardQuery = dbContext.FlashcardAnalyticRecords.Find(flashcardFilter);
                
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    switch (request.SortBy.ToLowerInvariant())
                    {
                        case "priority":
                            flashcardQuery = flashcardQuery.SortByDescending(r => r.ReviewPriority);
                            break;
                        case "difficulty":
                            flashcardQuery = flashcardQuery.SortByDescending(r => r.AccuracyRate);
                            break;
                        case "lastview":
                            flashcardQuery = flashcardQuery.SortByDescending(r => r.LastViewDate);
                            break;
                        case "nextreview":
                            flashcardQuery = flashcardQuery.SortBy(r => r.NextScheduledReview);
                            break;
                        default:
                            flashcardQuery = flashcardQuery.SortByDescending(r => r.LastUpdated);
                            break;
                    }
                }
                else
                {
                    flashcardQuery = flashcardQuery.SortByDescending(r => r.LastUpdated);
                }
                
                // Apply limit if specified
                if (request.MaxResults.HasValue && request.MaxResults.Value > 0)
                {
                    flashcardQuery = flashcardQuery.Limit(request.MaxResults.Value);
                }

                var flashcardAnalytics = await flashcardQuery.ToListAsync(cancellationToken);

                // Map to DTOs with improved handling of serialized fields
                response.FlashcardAnalytics = flashcardAnalytics.Select(fa => new FlashcardAnalyticDto
                {
                    FlashcardId = fa.FlashcardId,
                    ViewCount = fa.ViewCount,
                    FlipCount = fa.FlipCount,
                    TotalTimeSpentMs = fa.TotalTimeSpentMs,
                    TotalQuestions = fa.TotalQuestions,
                    CorrectAnswers = fa.CorrectAnswers,
                    AccuracyRate = fa.AccuracyRate,
                    AverageAnswerTimeMs = fa.AverageAnswerTimeMs,
                    LastViewDate = fa.LastViewDate,
                    NextScheduledReview = fa.NextScheduledReview,
                    RepetitionNumber = fa.RepetitionNumber,
                    EaseFactor = fa.EaseFactor,
                    IntervalDays = fa.IntervalDays,
                    LearningEfficiencyScore = fa.LearningEfficiencyScore,
                    ForgettingIndex = fa.ForgettingIndex,
                    ReviewPriority = fa.ReviewPriority,
                    PredictedForgettingDate = fa.PredictedForgettingDate,
                    StudyContexts = DeserializeJson<Dictionary<string, int>>(fa.StudyContexts),
                    DailyViewCounts = DeserializeJson<Dictionary<string, int>>(fa.DailyViewCounts),
                    DifficultyScore = flashcardAnalyzeService.CalculateDifficultyScore(fa),
                    OptimizationScore = flashcardAnalyzeService.CalculateOptimizationScore(fa),
                    TimeToMastery = flashcardAnalyzeService.CalculateTimeToMastery(fa)
                }).ToList();

                // Get user learning patterns with improved handling of serialized fields
                var userPattern = await dbContext.UserLearningPatternRecords
                    .Find(p => p.UserId == request.UserId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (userPattern != null)
                {
                    response.LearningPattern = new UserLearningPatternDto
                    {
                        StudyFrequency = userPattern.StudyFrequency,
                        CurrentStreak = userPattern.CurrentStreak,
                        LongestStreak = userPattern.LongestStreak,
                        LastStudyDate = userPattern.LastStudyDate,
                        OptimalStudyHour = userPattern.OptimalStudyHour,
                        MostFrequentStudyDay = userPattern.MostFrequentStudyDay,
                        PrimaryStudyContext = userPattern.PrimaryStudyContext,
                        StudyHours = DeserializeJson<Dictionary<int, int>>(userPattern.StudyHours),
                        EffectiveStudyHours = DeserializeJson<Dictionary<int, int>>(userPattern.EffectiveStudyHours),
                        StudyDaysOfWeek = DeserializeJson<Dictionary<int, int>>(userPattern.StudyDaysOfWeek),
                        StudyContexts = DeserializeJson<Dictionary<string, int>>(userPattern.StudyContexts),
                        StudyDaysInYear = DeserializeJson<Dictionary<string, bool>>(userPattern.StudyDaysInYear),
                        LearningSegment = flashcardAnalyzeService.GetLearningSegment(userPattern),
                        AverageOptimizationScore = flashcardAnalyzeService.CalculateAverageOptimizationScore(response.FlashcardAnalytics)
                    };
                }

                // Only get session details if requested
                if (request.IncludeSessionDetails)
                {
                    // Get recent sessions
                    var sessionFilter = Builders<SessionAnalyticRecord>.Filter.Eq(s => s.UserId, request.UserId);
                    if (request.StartDate.HasValue)
                    {
                        sessionFilter = sessionFilter & Builders<SessionAnalyticRecord>.Filter.Gte(s => s.StartTime, request.StartDate.Value);
                    }
                    if (request.EndDate.HasValue)
                    {
                        sessionFilter = sessionFilter & Builders<SessionAnalyticRecord>.Filter.Lte(s => s.EndTime, request.EndDate.Value);
                    }

                    var recentSessions = await dbContext.SessionAnalyticRecords
                        .Find(sessionFilter)
                        .SortByDescending(s => s.EndTime)
                        .Limit(10)
                        .ToListAsync(cancellationToken);

                    response.RecentSessions = recentSessions.Select(s => new SessionAnalyticDto
                    {
                        SessionId = s.Id,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        TotalTimeSpentMs = s.TotalTimeSpentMs,
                        CardsStudied = s.CardsStudied,
                        CorrectAnswers = s.CorrectAnswers,
                        AccuracyRate = s.AccuracyRate,
                        AverageTimePerCardMs = s.AverageTimePerCardMs,
                        StudyMode = s.StudyMode,
                        StudyContext = s.StudyContext,
                        FlashcardIds = DeserializeJson<List<Guid>>(s.FlashcardIds),
                        TimeOfDay = s.TimeOfDay,
                        DayOfWeek = s.DayOfWeek
                    }).ToList();
                }

                // Calculate enhanced summary
                if (flashcardAnalytics.Any())
                {
                    var now = DateTime.UtcNow;
                    response.Summary = new AnalyticsSummaryDto
                    {
                        TotalFlashcardsStudied = flashcardAnalytics.Count,
                        TotalStudySessions = response.RecentSessions?.Count ?? 0,
                        TotalTimeSpentMs = flashcardAnalytics.Sum(fa => fa.TotalTimeSpentMs),
                        AverageAccuracy = flashcardAnalytics.Average(fa => fa.AccuracyRate),
                        AverageEfficiencyScore = flashcardAnalytics.Average(fa => fa.LearningEfficiencyScore),
                        DifficultFlashcards = flashcardAnalytics
                            .Where(fa => flashcardAnalyzeService.CalculateDifficultyScore(fa) > 5)
                            .OrderByDescending(fa => flashcardAnalyzeService.CalculateDifficultyScore(fa))
                            .Take(5)
                            .Select(fa => fa.FlashcardId)
                            .ToList(),
                        MasteredFlashcards = flashcardAnalytics
                            .Where(fa => fa.AccuracyRate > 90 && fa.ViewCount > 3)
                            .OrderByDescending(fa => fa.AccuracyRate)
                            .Take(5)
                            .Select(fa => fa.FlashcardId)
                            .ToList(),
                        DueForReviewFlashcards = flashcardAnalytics
                            .Where(fa => fa.NextScheduledReview.HasValue && fa.NextScheduledReview.Value <= now)
                            .OrderBy(fa => fa.NextScheduledReview)
                            .Take(5)
                            .Select(fa => fa.FlashcardId)
                            .ToList(),
                        OverdueFlashcards = flashcardAnalytics.Count(fa => 
                            fa.NextScheduledReview.HasValue && fa.NextScheduledReview.Value < now),
                        DueFlashcardsToday = flashcardAnalytics.Count(fa => 
                            fa.NextScheduledReview.HasValue && 
                            fa.NextScheduledReview.Value >= now && 
                            fa.NextScheduledReview.Value <= now.AddDays(1)),
                        RetentionRate = flashcardAnalyzeService.CalculateRetentionRate(flashcardAnalytics),
                        AverageTimePerCardMs = flashcardAnalytics.Average(fa => fa.TotalTimeSpentMs / Math.Max(1, fa.ViewCount))
                    };
                }

                // Generate trend analysis
                response.Trends = flashcardAnalyzeService.CalculateTrends(response.RecentSessions, flashcardAnalytics);
                
                // Generate recommendations
                response.Recommendations = flashcardAnalyzeService.GenerateRecommendations(response);
                
                //Formatting some fields
                formattingService.FormatUserAnalytics(response);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving flashcard analytics for user {UserId}", request.UserId);
                throw;
            }
        }

        private T DeserializeJson<T>(string json) where T : new()
        {
            if (string.IsNullOrEmpty(json))
                return new T();
                
            try
            {
                return JsonConvert.DeserializeObject<T>(json) ?? new T();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error deserializing JSON: {Json}", json);
                return new T();
            }
        }
    }
}
