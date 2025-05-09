using Application.Common.Kafka;
using Newtonsoft.Json;
using SharedProject.Models;
using System.Collections.Concurrent;
using Application.Features.AnalyseFeature.Supports;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.AnalyseFeature.EventHandler
{
    public class FlashcardAnalyzeConsumer(
        IConfiguration configuration,
        ILogger<FlashcardAnalyzeConsumer> logger,
        IServiceProvider serviceProvider,
        string topicName = "flashcard_analyze_data",
        string groupId = "flashcard_analyze_group")
        : KafkaConsumerBaseBatch<UserAnalyseFlashcardMessageModel>(configuration, logger, serviceProvider, topicName,
            groupId)
    {
        // ConcurrentDictionary để đảm bảo thread-safety
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, FlashcardAnalytics>>
            _userFlashcardAnalytics
                = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, FlashcardAnalytics>>();

        // Lưu trữ thông tin phiên học tập
        private readonly ConcurrentDictionary<Guid, SessionAnalytics> _sessionAnalytics
            = new ConcurrentDictionary<Guid, SessionAnalytics>();

        // Lưu trữ xu hướng học tập theo thời gian
        private readonly ConcurrentDictionary<Guid, UserLearningPatterns> _userLearningPatterns
            = new ConcurrentDictionary<Guid, UserLearningPatterns>();

        protected override async Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider)
        {
            try
            {
                // Chuyển đổi messages từ JSON thành UserAnalyseFlashcardMessageModel
                var flashcardEvents = messages
                    .Select(msg =>
                    {
                        try
                        {
                            return JsonConvert.DeserializeObject<UserAnalyseFlashcardMessageModel>(msg);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError($"Failed to deserialize message: {ex.Message}. Message: {msg}");
                            return null;
                        }
                    })
                    .Where(e => e != null && e.UserId.HasValue)
                    .ToList();

                if (!flashcardEvents.Any())
                {
                    logger.LogWarning("No valid flashcard events found in batch.");
                    return;
                }

                // Xử lý từng sự kiện flashcard
                foreach (var flashcardEvent in flashcardEvents)
                {
                    ProcessFlashcardEvent(flashcardEvent);
                }

                // Phân tích xu hướng học tập và hiệu suất dài hạn
                AnalyzeLearningPatterns(flashcardEvents);

                // Tính toán các chỉ số học tập nâng cao
                CalculateAdvancedMetrics();

                // Gửi dữ liệu phân tích đã xử lý (lưu vào DB)
                await PersistAnalytics(serviceProvider, flashcardEvents!);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing flashcard batch: {ex.Message}");
                throw;
            }
        }

        private void ProcessFlashcardEvent(UserAnalyseFlashcardMessageModel flashcardEvent)
        {
            if (flashcardEvent == null || !flashcardEvent.UserId.HasValue) return;

            var userId = flashcardEvent.UserId.Value;

            // Đảm bảo có dictionary cho user này
            var userAnalytics = _userFlashcardAnalytics.GetOrAdd(userId,
                _ => new ConcurrentDictionary<Guid, FlashcardAnalytics>());

            // Cập nhật thông tin phiên học tập
            if (flashcardEvent.SessionId.HasValue)
            {
                UpdateSessionAnalytics(flashcardEvent);
            }

            // Tùy thuộc vào loại học tập
            if (flashcardEvent.StudyMode == "Flashcard" && flashcardEvent.DataLearningMode?.FlashcardId != null)
            {
                ProcessFlashcardLearningMode(flashcardEvent, userAnalytics);
            }
            else if ((flashcardEvent.StudyMode == "Learn" || flashcardEvent.StudyMode == "Test")
                     && flashcardEvent.DataStudyMode?.FlashcardId != null)
            {
                ProcessStudyTestMode(flashcardEvent, userAnalytics);
            }
            else
            {
                logger.LogWarning($"Unrecognized study mode or missing flashcard ID: {flashcardEvent.StudyMode}");
            }

            // Cập nhật mẫu học tập của người dùng
            UpdateUserLearningPatterns(flashcardEvent);
        }

        private void ProcessFlashcardLearningMode(UserAnalyseFlashcardMessageModel flashcardEvent,
            ConcurrentDictionary<Guid, FlashcardAnalytics> userAnalytics)
        {
            var flashcardId = flashcardEvent.DataLearningMode.FlashcardId.Value;
            var data = flashcardEvent.DataLearningMode;

            // Lấy hoặc tạo thông tin phân tích cho flashcard này
            var analytics = userAnalytics.GetOrAdd(flashcardId, _ => new FlashcardAnalytics());

            // Cập nhật thời gian học tập
            if (data.TimeSpentMs.HasValue)
            {
                Interlocked.Add(ref analytics._totalTimeSpentMs, data.TimeSpentMs.Value);
            }

            // Cập nhật số lần xem
            Interlocked.Increment(ref analytics._viewCount);

            // Cập nhật số lần lật thẻ
            if (data.FlipCount.HasValue)
            {
                Interlocked.Add(ref analytics._flipCount, data.FlipCount.Value);
            }

            // Cập nhật thời gian trả lời trung bình
            if (data.AverageAnswerTimeEachCardMs.HasValue)
            {
                analytics.UpdateAverageAnswerTime(data.AverageAnswerTimeEachCardMs.Value);
            }

            // Cập nhật tỷ lệ chính xác
            if (data.AccuracyRate.HasValue)
            {
                analytics.UpdateAccuracyRate(data.AccuracyRate.Value);
            }

            // Cập nhật đánh giá bản thân
            if (data.SelfRating.HasValue)
            {
                analytics.UpdateSelfRating(data.SelfRating.Value);
            }

            // Cập nhật lượt xem theo ngày
            var today = DateTime.UtcNow.Date;
            analytics.DailyViewCounts[today] = analytics.DailyViewCounts.GetValueOrDefault(today, 0) + 1;

            // Cập nhật lần xem cuối cùng
            analytics.LastViewDate = flashcardEvent.TimeStamp;

            // Cập nhật phương pháp học tập
            if (!string.IsNullOrEmpty(flashcardEvent.StudyContext))
            {
                analytics.UpdateStudyContext(flashcardEvent.StudyContext);
            }
        }

        private void ProcessStudyTestMode(UserAnalyseFlashcardMessageModel flashcardEvent,
            ConcurrentDictionary<Guid, FlashcardAnalytics> userAnalytics)
        {
            var flashcardId = flashcardEvent.DataStudyMode.FlashcardId.Value;
            var data = flashcardEvent.DataStudyMode;

            // Lấy hoặc tạo thông tin phân tích cho flashcard này
            var analytics = userAnalytics.GetOrAdd(flashcardId, _ => new FlashcardAnalytics());

            // Cập nhật thời gian học tập
            if (data.TimeSpentMs.HasValue)
            {
                Interlocked.Add(ref analytics._totalTimeSpentMs, data.TimeSpentMs.Value);
            }

            // Cập nhật số lần xem
            if (data.ViewCount.HasValue)
            {
                Interlocked.Add(ref analytics._viewCount, data.ViewCount.Value);
            }
            else
            {
                Interlocked.Increment(ref analytics._viewCount);
            }

            // Cập nhật số câu hỏi và câu trả lời đúng
            if (data.TotalQuestionsInRound.HasValue)
            {
                Interlocked.Add(ref analytics._totalQuestions, data.TotalQuestionsInRound.Value);
            }

            if (data.AnswerCorrect.HasValue && data.AnswerCorrect.Value > 0)
            {
                Interlocked.Add(ref analytics._correctAnswers, data.AnswerCorrect.Value);
            }

            // Cập nhật tỷ lệ chính xác
            analytics.CalculateAccuracyRate();

            // Cập nhật thời gian trả lời trung bình
            if (data.AverageAnswerTimeEachQuestionMs.HasValue)
            {
                analytics.UpdateAverageAnswerTime(data.AverageAnswerTimeEachQuestionMs.Value);
            }

            // Cập nhật lần xem cuối cùng và lần ôn tập tiếp theo
            analytics.LastViewDate = flashcardEvent.TimeStamp;
            if (data.NextScheduledReview.HasValue)
            {
                analytics.NextScheduledReview = data.NextScheduledReview;
            }

            // Cập nhật thông tin về spaced repetition
            if (data.RepetitionNumber.HasValue)
            {
                analytics.RepetitionNumber = Math.Max(analytics.RepetitionNumber ?? 0, data.RepetitionNumber.Value);
            }

            if (data.EaseFactor.HasValue)
            {
                analytics.EaseFactor = data.EaseFactor;
            }

            if (data.IntervalDays.HasValue)
            {
                analytics.IntervalDays = data.IntervalDays;
            }

            // Cập nhật lượt xem theo ngày
            var today = DateTime.UtcNow.Date;
            analytics.DailyViewCounts[today] = analytics.DailyViewCounts.GetValueOrDefault(today, 0) + 1;
            // Cập nhật trạng thái ôn tập
            if (data.IsReview.HasValue)
            {
                analytics.UpdateReviewStatus(data.IsReview.Value, flashcardEvent.TimeStamp);
            }
        }

        private void UpdateSessionAnalytics(UserAnalyseFlashcardMessageModel flashcardEvent)
        {
            if (!flashcardEvent.SessionId.HasValue) return;

            var sessionId = flashcardEvent.SessionId.Value;
            var userId = flashcardEvent.UserId.Value;

            var sessionAnalytics = _sessionAnalytics.GetOrAdd(sessionId,
                _ => new SessionAnalytics
                {
                    UserId = userId,
                    StartTime = flashcardEvent.TimeStamp,
                    StudyMode = flashcardEvent.StudyMode,
                    StudyContext = flashcardEvent.StudyContext
                });

            // Cập nhật thời gian kết thúc phiên
            sessionAnalytics.EndTime = flashcardEvent.TimeStamp;

            // Cập nhật số flashcard đã học trong phiên
            if (flashcardEvent.DataStudyMode?.CardsStudiedInSession.HasValue == true)
            {
                sessionAnalytics.CardsStudied = flashcardEvent.DataStudyMode.CardsStudiedInSession.Value;
            }

            // Cập nhật số câu trả lời đúng trong phiên
            if (flashcardEvent.DataStudyMode?.CorrectAnswersInSession.HasValue == true)
            {
                sessionAnalytics.CorrectAnswers = flashcardEvent.DataStudyMode.CorrectAnswersInSession.Value;
            }

            // Cập nhật thời gian học tập cho phiên này
            if (flashcardEvent.DataStudyMode?.TimeSpentMs.HasValue == true)
            {
                sessionAnalytics.TotalTimeSpentMs += flashcardEvent.DataStudyMode.TimeSpentMs.Value;
            }
            else if (flashcardEvent.DataLearningMode?.TimeSpentMs.HasValue == true)
            {
                sessionAnalytics.TotalTimeSpentMs += flashcardEvent.DataLearningMode.TimeSpentMs.Value;
            }

            // Cập nhật danh sách flashcard đã học trong phiên
            Guid? flashcardId = null;
            if (flashcardEvent.DataStudyMode?.FlashcardId.HasValue == true)
            {
                flashcardId = flashcardEvent.DataStudyMode.FlashcardId.Value;
            }
            else if (flashcardEvent.DataLearningMode?.FlashcardId.HasValue == true)
            {
                flashcardId = flashcardEvent.DataLearningMode.FlashcardId.Value;
            }

            if (flashcardId.HasValue && !sessionAnalytics.FlashcardIds.Contains(flashcardId.Value))
            {
                sessionAnalytics.FlashcardIds.Add(flashcardId.Value);
            }

            // Tính toán tỷ lệ chính xác cho phiên
            if (sessionAnalytics.CardsStudied > 0)
            {
                sessionAnalytics.AccuracyRate =
                    (double)sessionAnalytics.CorrectAnswers / sessionAnalytics.CardsStudied * 100;
            }

            // Tính toán thời gian trung bình cho mỗi flashcard
            if (sessionAnalytics.FlashcardIds.Count > 0)
            {
                sessionAnalytics.AverageTimePerCardMs =
                    sessionAnalytics.TotalTimeSpentMs / sessionAnalytics.FlashcardIds.Count;
            }

            // Cập nhật thông tin thời gian của ngày
            if (flashcardEvent.TimeOfDay.HasValue)
            {
                sessionAnalytics.TimeOfDay = flashcardEvent.TimeOfDay.Value;
            }

            if (flashcardEvent.DayOfWeek.HasValue)
            {
                sessionAnalytics.DayOfWeek = flashcardEvent.DayOfWeek.Value;
            }
        }

        private void UpdateUserLearningPatterns(UserAnalyseFlashcardMessageModel flashcardEvent)
        {
            if (!flashcardEvent.UserId.HasValue) return;

            var userId = flashcardEvent.UserId.Value;
            var userPatterns = _userLearningPatterns.GetValueOrDefault(userId) ?? new UserLearningPatterns();

            // Ensure dictionaries are initialized
            userPatterns.StudyHours ??= new Dictionary<int, int>();
            userPatterns.EffectiveStudyHours ??= new Dictionary<int, int>();
            userPatterns.StudyDaysOfWeek ??= new Dictionary<int, int>();
            userPatterns.StudyContexts ??= new Dictionary<string, int>();
            userPatterns.StudyDaysInYear ??= new Dictionary<int, int>();

            // Update study hours
            if (flashcardEvent.TimeOfDay.HasValue)
            {
                var hour = (int)flashcardEvent.TimeOfDay.Value.TotalHours;
                userPatterns.StudyHours[hour] = userPatterns.StudyHours.GetValueOrDefault(hour) + 1;

                bool isCorrect = false;
                if (flashcardEvent.DataStudyMode?.AnswerCorrect.HasValue == true &&
                    flashcardEvent.DataStudyMode.AnswerCorrect.Value > 0)
                {
                    isCorrect = true;
                }
                else if (flashcardEvent.DataLearningMode?.AccuracyRate.HasValue == true &&
                         flashcardEvent.DataLearningMode.AccuracyRate.Value > 0.7)
                {
                    isCorrect = true;
                }

                if (isCorrect)
                {
                    userPatterns.EffectiveStudyHours[hour] = userPatterns.EffectiveStudyHours.GetValueOrDefault(hour) + 1;
                }
            }

            // Update study days of week (using int to match UserLearningPatternRecord)
            if (flashcardEvent.DayOfWeek.HasValue)
            {
                var day = (int)flashcardEvent.DayOfWeek.Value; // Convert DayOfWeek to int (0-6)
                userPatterns.StudyDaysOfWeek[day] = userPatterns.StudyDaysOfWeek.GetValueOrDefault(day, 0) + 1;
            }

            // Update study context
            if (!string.IsNullOrEmpty(flashcardEvent.StudyContext))
            {
                userPatterns.StudyContexts[flashcardEvent.StudyContext] = 
                    userPatterns.StudyContexts.GetValueOrDefault(flashcardEvent.StudyContext) + 1;
            }

            // Update study streak
            var today = DateTime.UtcNow.Date;
            if (!userPatterns.LastStudyDate.HasValue)
            {
                userPatterns.LastStudyDate = today;
                userPatterns.CurrentStreak = 1;
            }
            else
            {
                var dayDiff = (today - userPatterns.LastStudyDate.Value).TotalDays;
                if (Math.Abs(dayDiff - 1) < 0.1) // Approximately 1 day apart
                {
                    userPatterns.CurrentStreak++;
                    userPatterns.LongestStreak = Math.Max(userPatterns.LongestStreak, userPatterns.CurrentStreak);
                }
                else if (dayDiff > 1) // Streak broken
                {
                    userPatterns.CurrentStreak = 1;
                }
                userPatterns.LastStudyDate = today;
            }

            // Update study days in year
            var dayOfYear = today.DayOfYear;
            userPatterns.StudyDaysInYear[dayOfYear] = userPatterns.StudyDaysInYear.GetValueOrDefault(dayOfYear) + 1;

            // Update derived properties
            userPatterns.StudyFrequency = userPatterns.StudyDaysInYear.Count; // Number of unique study days
            userPatterns.OptimalStudyHour = userPatterns.EffectiveStudyHours.Any() 
                ? userPatterns.EffectiveStudyHours.OrderByDescending(kv => kv.Value).First().Key 
                : (int?)null;
            userPatterns.MostFrequentStudyDay = userPatterns.StudyDaysOfWeek.Any() 
                ? (int?)userPatterns.StudyDaysOfWeek.OrderByDescending(kv => kv.Value).First().Key 
                : (int?)null;
            userPatterns.PrimaryStudyContext = userPatterns.StudyContexts.Any() 
                ? userPatterns.StudyContexts.OrderByDescending(kv => kv.Value).First().Key 
                : null;

            // Store updated patterns back in the dictionary
            _userLearningPatterns[userId] = userPatterns;
        }

        private void AnalyzeLearningPatterns(List<UserAnalyseFlashcardMessageModel> flashcardEvents)
        {
            // Nhóm các sự kiện theo UserId
            var userGroups = flashcardEvents
                .Where(e => e.UserId.HasValue)
                .GroupBy(e => e.UserId.Value);

            foreach (var userGroup in userGroups)
            {
                var userId = userGroup.Key;
                var userPatterns = _userLearningPatterns.GetOrAdd(userId, _ => new UserLearningPatterns());

                // Phân tích thời gian học tập tối ưu
                if (userPatterns.StudyHours.Count > 0 && userPatterns.EffectiveStudyHours.Count > 0)
                {
                    var totalStudyCount = userPatterns.StudyHours.Values.Sum();

                    // Tìm giờ học hiệu quả nhất (tỷ lệ câu trả lời đúng cao nhất)
                    var bestHour = userPatterns.EffectiveStudyHours
                        .OrderByDescending(h =>
                        {
                            if (userPatterns.StudyHours.TryGetValue(h.Key, out var totalCount) && totalCount > 0)
                            {
                                return (double)h.Value / totalCount;
                            }

                            return 0.0;
                        })
                        .FirstOrDefault();

                    if (bestHour.Key != 0 || bestHour.Value > 0)
                    {
                        userPatterns.OptimalStudyHour = bestHour.Key;
                    }
                }

                // Phân tích ngày học tập tốt nhất trong tuần
                if (userPatterns.StudyDaysOfWeek.Count > 0)
                {
                    var bestDay = userPatterns.StudyDaysOfWeek
                        .OrderByDescending(d => d.Value)
                        .FirstOrDefault();

                    userPatterns.MostFrequentStudyDay = (int?)bestDay.Key;
                }

                // Phân tích tần suất học tập
                if (userPatterns.StudyDaysInYear.Count > 0)
                {
                    userPatterns.StudyFrequency = (double)userPatterns.StudyDaysInYear.Count / 365;
                }

                // Phân tích bối cảnh học tập phổ biến
                if (userPatterns.StudyContexts.Count > 0)
                {
                    var mostCommonContext = userPatterns.StudyContexts
                        .OrderByDescending(c => c.Value)
                        .FirstOrDefault();

                    userPatterns.PrimaryStudyContext = mostCommonContext.Key;
                }
            }
        }

        private void CalculateAdvancedMetrics()
        {
            // Xử lý cho từng người dùng
            foreach (var userKvp in _userFlashcardAnalytics)
            {
                var userId = userKvp.Key;
                var flashcardAnalytics = userKvp.Value;

                // Nếu chúng ta có dữ liệu mẫu học tập cho người dùng này
                if (_userLearningPatterns.TryGetValue(userId, out var userPatterns))
                {
                    // Tính toán các chỉ số nâng cao cho từng flashcard
                    foreach (var flashcardKvp in flashcardAnalytics)
                    {
                        var flashcardId = flashcardKvp.Key;
                        var analytics = flashcardKvp.Value;

                        // Tính toán chỉ số hiệu quả học tập
                        CalculateLearningEfficiencyScore(analytics);

                        // Tính toán chỉ số quên (forgetting index)
                        CalculateForgettingIndex(analytics);

                        // Tính toán độ ưu tiên ôn tập
                        CalculateReviewPriority(analytics, userPatterns);

                        // Dự đoán thời gian quên
                        PredictForgettingTime(analytics);
                    }
                }
            }
        }

        private void CalculateLearningEfficiencyScore(FlashcardAnalytics analytics)
        {
            // Hiệu quả học tập = Tỷ lệ chính xác / (Thời gian trung bình / Độ phức tạp)
            // Chúng ta giả định độ phức tạp = 1 nếu không có thông tin

            double complexityFactor = 1.0;
            if (analytics.RepetitionNumber.HasValue && analytics.RepetitionNumber > 0)
            {
                complexityFactor = Math.Min(1.0, 1.0 / analytics.RepetitionNumber.Value);
            }

            if (analytics.AccuracyRate > 0 && analytics.AverageAnswerTimeMs > 0)
            {
                // Normalize thời gian trả lời (giả sử 10 giây là tiêu chuẩn)
                double normalizedTime = Math.Min(1.0, 10000.0 / analytics.AverageAnswerTimeMs);

                // Tính điểm hiệu quả (từ 0-100)
                analytics.LearningEfficiencyScore =
                    (analytics.AccuracyRate / 100.0) * normalizedTime / complexityFactor * 100;
            }
        }

        private void CalculateForgettingIndex(FlashcardAnalytics analytics)
        {
            // Chỉ số quên dựa trên đường cong Ebbinghaus
            // f(t) = e^(-t/S) trong đó S là độ mạnh của ký ức và t là thời gian đã trôi qua

            if (analytics.LastViewDate.HasValue)
            {
                var daysSinceLastView = (DateTime.UtcNow - analytics.LastViewDate.Value).TotalDays;

                // Tính toán độ mạnh của ký ức (S)
                double memoryStrength = 1.0;
                if (analytics.RepetitionNumber.HasValue && analytics.RepetitionNumber > 0)
                {
                    memoryStrength = Math.Log(analytics.RepetitionNumber.Value + 1, 2);
                }

                if (analytics.EaseFactor.HasValue && analytics.EaseFactor > 0)
                {
                    memoryStrength *= analytics.EaseFactor.Value;
                }

                // Tính chỉ số quên (0-100, cao là khả năng quên cao)
                analytics.ForgettingIndex = 100 * (1 - Math.Exp(-daysSinceLastView / memoryStrength));
            }
        }

        private void CalculateReviewPriority(FlashcardAnalytics analytics, UserLearningPatterns userPatterns)
        {
            // Độ ưu tiên ôn tập = Chỉ số quên * (1 - Hiệu quả học tập/100) * Hệ số quan trọng

            double importanceFactor = 1.0;

            // Nếu có thông tin về bối cảnh học tập
            if (analytics.StudyContextCounts.Count > 0 && !string.IsNullOrEmpty(userPatterns.PrimaryStudyContext))
            {
                if (analytics.StudyContextCounts.TryGetValue(userPatterns.PrimaryStudyContext, out var count) &&
                    count > 0)
                {
                    // Tăng hệ số quan trọng nếu flashcard này thuộc bối cảnh học tập chính
                    importanceFactor = 1.2;
                }
            }

            // Tính toán độ ưu tiên ôn tập
            if (analytics.ForgettingIndex > 0)
            {
                double learningEfficiency = Math.Max(1, analytics.LearningEfficiencyScore) / 100.0;
                analytics.ReviewPriority = analytics.ForgettingIndex * (1 - learningEfficiency) * importanceFactor;
            }
        }

        private void PredictForgettingTime(FlashcardAnalytics analytics)
        {
            // Dự đoán thời gian quên dựa trên khoảng thời gian giữa các lần ôn tập và độ mạnh ký ức

            if (analytics.IntervalDays.HasValue && analytics.IntervalDays > 0)
            {
                // Nếu đã có khoảng thời gian giữa các lần ôn tập
                double retentionThreshold = 0.7; // Dưới 70% là quên

                // Tính toán độ mạnh ký ức
                double memoryStrength = 1.0;
                if (analytics.RepetitionNumber.HasValue && analytics.RepetitionNumber > 0)
                {
                    memoryStrength = Math.Log(analytics.RepetitionNumber.Value + 1, 2);
                }

                if (analytics.EaseFactor.HasValue && analytics.EaseFactor > 0)
                {
                    memoryStrength *= analytics.EaseFactor.Value;
                }

                // Dự đoán số ngày cho đến khi quên (retention < threshold)
                double daysToForget = -memoryStrength * Math.Log(retentionThreshold);

                // Cộng với ngày xem cuối cùng để có thời điểm quên dự đoán
                if (analytics.LastViewDate.HasValue)
                {
                    analytics.PredictedForgettingDate =
                        analytics.LastViewDate.Value.AddDays(daysToForget);
                }
            }
            else if (analytics.LastViewDate.HasValue)
            {
                // Nếu không có thông tin về khoảng thời gian, dự đoán dựa trên đường cong quên cơ bản
                // Giả định 7 ngày cho lần đầu tiên
                analytics.PredictedForgettingDate = analytics.LastViewDate.Value.AddDays(7);
            }
        }
        private async Task PersistAnalytics(IServiceProvider serviceProvider, List<UserAnalyseFlashcardMessageModel> events)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<AnalyseDbContext>();

            if (dbContext == null)
            {
                logger.LogError("DbContext is null. Unable to persist analytics data.");
                return;
            }

            // Get MongoDB collections
            var flashcardCollection = dbContext.FlashcardAnalyticRecords;
            var sessionCollection = dbContext.SessionAnalyticRecords;
            var patternCollection = dbContext.UserLearningPatternRecords;

            // 1. Flashcard Analytics
            var flashcardAnalyticsRecords = new List<FlashcardAnalyticRecord>();
            foreach (var userAnalytics in _userFlashcardAnalytics)
            {
                foreach (var flashcardAnalytics in userAnalytics.Value)
                {
                    var analytics = flashcardAnalytics.Value;
                    var analyticRecord = new FlashcardAnalyticRecord
                    {
                        Id = Guid.NewGuid(),
                        UserId = userAnalytics.Key,
                        FlashcardId = flashcardAnalytics.Key,
                        TotalTimeSpentMs = analytics.TotalTimeSpentMs,
                        ViewCount = analytics.ViewCount,
                        FlipCount = analytics.FlipCount,
                        TotalQuestions = analytics._totalQuestions,
                        CorrectAnswers = analytics._correctAnswers,
                        DailyViewCounts = JsonConvert.SerializeObject(analytics.DailyViewCounts),
                        AccuracyRate = analytics.AccuracyRate,
                        AverageAnswerTimeMs = analytics.AverageAnswerTimeMs,
                        LastViewDate = analytics.LastViewDate,
                        NextScheduledReview = analytics.NextScheduledReview,
                        RepetitionNumber = analytics.RepetitionNumber,
                        EaseFactor = analytics.EaseFactor,
                        IntervalDays = analytics.IntervalDays,
                        LearningEfficiencyScore = analytics.LearningEfficiencyScore,
                        ForgettingIndex = analytics.ForgettingIndex,
                        ReviewPriority = analytics.ReviewPriority,
                        PredictedForgettingDate = analytics.PredictedForgettingDate,
                        StudyContexts = JsonConvert.SerializeObject(analytics.StudyContextCounts),
                        LastUpdated = DateTime.UtcNow
                    };
                    flashcardAnalyticsRecords.Add(analyticRecord);
                }
            }

            // Bulk upsert for Flashcard Analytics
            foreach (var record in flashcardAnalyticsRecords)
            {
                var filter = Builders<FlashcardAnalyticRecord>.Filter
                    .Eq(r => r.UserId, record.UserId) & Builders<FlashcardAnalyticRecord>.Filter
                    .Eq(r => r.FlashcardId, record.FlashcardId);

                var update = Builders<FlashcardAnalyticRecord>.Update
                    .SetOnInsert(r => r.Id, record.Id)
                    .Set(r => r.TotalTimeSpentMs, record.TotalTimeSpentMs)
                    .Set(r => r.ViewCount, record.ViewCount)
                    .Set(r => r.FlipCount, record.FlipCount)
                    .Set(r => r.TotalQuestions, record.TotalQuestions)
                    .Set(r => r.CorrectAnswers, record.CorrectAnswers)
                    .Set(r => r.DailyViewCounts, record.DailyViewCounts)
                    .Set(r => r.AccuracyRate, record.AccuracyRate)
                    .Set(r => r.AverageAnswerTimeMs, record.AverageAnswerTimeMs)
                    .Set(r => r.LastViewDate, record.LastViewDate)
                    .Set(r => r.NextScheduledReview, record.NextScheduledReview)
                    .Set(r => r.RepetitionNumber, record.RepetitionNumber)
                    .Set(r => r.EaseFactor, record.EaseFactor)
                    .Set(r => r.IntervalDays, record.IntervalDays)
                    .Set(r => r.LearningEfficiencyScore, record.LearningEfficiencyScore)
                    .Set(r => r.ForgettingIndex, record.ForgettingIndex)
                    .Set(r => r.ReviewPriority, record.ReviewPriority)
                    .Set(r => r.PredictedForgettingDate, record.PredictedForgettingDate)
                    .Set(r => r.StudyContexts, record.StudyContexts)
                    .Set(r => r.LastUpdated, DateTime.UtcNow);

                await flashcardCollection.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<FlashcardAnalyticRecord> { IsUpsert = true });
            }

            // 2. Session Analytics
            var sessionRecords = new List<SessionAnalyticRecord>();
            foreach (var sessionAnalytics in _sessionAnalytics)
            {
                var session = sessionAnalytics.Value;
                var sessionRecord = new SessionAnalyticRecord
                {
                    Id = sessionAnalytics.Key,
                    UserId = session.UserId,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    TotalTimeSpentMs = session.TotalTimeSpentMs,
                    CardsStudied = session.CardsStudied,
                    CorrectAnswers = session.CorrectAnswers,
                    AccuracyRate = session.AccuracyRate,
                    AverageTimePerCardMs = session.AverageTimePerCardMs,
                    StudyMode = session.StudyMode,
                    StudyContext = session.StudyContext,
                    FlashcardIds = JsonConvert.SerializeObject(session.FlashcardIds),
                    TimeOfDay = session.TimeOfDay,
                    DayOfWeek = (int?)session.DayOfWeek
                };
                sessionRecords.Add(sessionRecord);
            }

            // Bulk upsert for Session Analytics
            foreach (var record in sessionRecords)
            {
                var filter = Builders<SessionAnalyticRecord>.Filter
                    .Eq(r => r.Id, record.Id);

                var update = Builders<SessionAnalyticRecord>.Update
                    .SetOnInsert(r => r.Id, record.Id)
                    .Set(r => r.UserId, record.UserId)
                    .Set(r => r.StartTime, record.StartTime)
                    .Set(r => r.EndTime, record.EndTime)
                    .Set(r => r.TotalTimeSpentMs, record.TotalTimeSpentMs)
                    .Set(r => r.CardsStudied, record.CardsStudied)
                    .Set(r => r.CorrectAnswers, record.CorrectAnswers)
                    .Set(r => r.AccuracyRate, record.AccuracyRate)
                    .Set(r => r.AverageTimePerCardMs, record.AverageTimePerCardMs)
                    .Set(r => r.StudyMode, record.StudyMode)
                    .Set(r => r.StudyContext, record.StudyContext)
                    .Set(r => r.FlashcardIds, record.FlashcardIds)
                    .Set(r => r.TimeOfDay, record.TimeOfDay)
                    .Set(r => r.DayOfWeek, record.DayOfWeek);

                await sessionCollection.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<SessionAnalyticRecord> { IsUpsert = true });
            }

            // 3. User Learning Patterns
            var patternRecords = new List<UserLearningPatternRecord>();
            foreach (var userPattern in _userLearningPatterns)
            {
                var pattern = userPattern.Value;
                var patternRecord = new UserLearningPatternRecord
                {
                    UserId = userPattern.Key,
                    StudyFrequency = pattern.StudyFrequency,
                    CurrentStreak = pattern.CurrentStreak,
                    LongestStreak = pattern.LongestStreak,
                    LastStudyDate = pattern.LastStudyDate,
                    OptimalStudyHour = pattern.OptimalStudyHour,
                    MostFrequentStudyDay = pattern.MostFrequentStudyDay,
                    PrimaryStudyContext = pattern.PrimaryStudyContext,
                    StudyHours = JsonConvert.SerializeObject(pattern.StudyHours),
                    EffectiveStudyHours = JsonConvert.SerializeObject(pattern.EffectiveStudyHours),
                    StudyDaysOfWeek = JsonConvert.SerializeObject(pattern.StudyDaysOfWeek),
                    StudyContexts = JsonConvert.SerializeObject(pattern.StudyContexts),
                    StudyDaysInYear = JsonConvert.SerializeObject(pattern.StudyDaysInYear),
                    LastUpdated = DateTime.UtcNow
                };
                patternRecords.Add(patternRecord);
            }

            // Bulk upsert for User Learning Patterns
            foreach (var record in patternRecords)
            {
                var filter = Builders<UserLearningPatternRecord>.Filter
                    .Eq(r => r.UserId, record.UserId);

                var update = Builders<UserLearningPatternRecord>.Update
                    .SetOnInsert(r => r.UserId, record.UserId)
                    .Set(r => r.StudyFrequency, record.StudyFrequency)
                    .Set(r => r.CurrentStreak, record.CurrentStreak)
                    .Set(r => r.LongestStreak, record.LongestStreak)
                    .Set(r => r.LastStudyDate, record.LastStudyDate)
                    .Set(r => r.OptimalStudyHour, record.OptimalStudyHour)
                    .Set(r => r.MostFrequentStudyDay, record.MostFrequentStudyDay)
                    .Set(r => r.PrimaryStudyContext, record.PrimaryStudyContext)
                    .Set(r => r.StudyHours, record.StudyHours)
                    .Set(r => r.EffectiveStudyHours, record.EffectiveStudyHours)
                    .Set(r => r.StudyDaysOfWeek, record.StudyDaysOfWeek)
                    .Set(r => r.StudyContexts, record.StudyContexts)
                    .Set(r => r.StudyDaysInYear, record.StudyDaysInYear)
                    .Set(r => r.LastUpdated, DateTime.UtcNow);

                await patternCollection.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<UserLearningPatternRecord> { IsUpsert = true });
            }
        }       
        
    }
}

public class FlashcardAnalytics
{
    // Thời gian tổng cộng dành cho flashcard (ms)
    public long _totalTimeSpentMs;
    // Tổng số lần xem
    public int _viewCount;
    // Tổng số lần lật thẻ
    public int _flipCount;
    // Tổng số câu hỏi
    public int _totalQuestions;
    // Tổng số câu trả lời đúng
    public int _correctAnswers;
    
    // Các thuộc tính phân tích
    public long TotalTimeSpentMs => _totalTimeSpentMs;
    public int ViewCount => _viewCount;
    public int FlipCount => _flipCount;
    public Dictionary<DateTime, int> DailyViewCounts { get; } = new Dictionary<DateTime, int>();
    public double AccuracyRate { get; private set; }
    public double AverageAnswerTimeMs { get; private set; }
    public int SessionCount { get; private set; }
    public Dictionary<string, int> StudyContextCounts { get; } = new Dictionary<string, int>();
    
    // Thông tin cho Spaced Repetition
    public DateTime? LastViewDate { get; set; }
    public DateTime? NextScheduledReview { get; set; }
    public int? RepetitionNumber { get; set; }
    public double? EaseFactor { get; set; }
    public int? IntervalDays { get; set; }
    
    // Các chỉ số phân tích nâng cao
    public double LearningEfficiencyScore { get; set; }
    public double ForgettingIndex { get; set; }
    public double ReviewPriority { get; set; }
    public DateTime? PredictedForgettingDate { get; set; }
    public DateTime? LastReviewDate { get; set; }
    
    public void UpdateAverageAnswerTime(double newAnswerTime)
    {
        if (AverageAnswerTimeMs <= 0)
        {
            AverageAnswerTimeMs = newAnswerTime;
        }
        else
        {
            // Cập nhật giá trị trung bình theo phương pháp trung bình có trọng số
            AverageAnswerTimeMs = (AverageAnswerTimeMs * 0.7) + (newAnswerTime * 0.3);
        }
    }
    
    public void UpdateAccuracyRate(double newAccuracyRate)
    {
        if (AccuracyRate <= 0)
        {
            AccuracyRate = newAccuracyRate;
        }
        else
        {
            // Cập nhật tỷ lệ chính xác với trọng số
            AccuracyRate = (AccuracyRate * 0.7) + (newAccuracyRate * 0.3);
        }
    }
    
    public void CalculateAccuracyRate()
    {
        if (_totalQuestions > 0)
        {
            AccuracyRate = (double)_correctAnswers / _totalQuestions * 100;
        }
    }
    
    public void UpdateSelfRating(int rating)
    {
        // Có thể lưu lịch sử đánh giá hoặc tính trung bình
    }
    
    public void UpdateStudyContext(string context)
    {
        if (!string.IsNullOrEmpty(context))
        {
            StudyContextCounts.TryGetValue(context, out int count);
            StudyContextCounts[context] = count + 1;
        }
    }
    
    public void UpdateReviewStatus(bool isReview, DateTime timestamp)
    {
        if (isReview)
        {
            LastReviewDate = timestamp;
        }
    }
}

