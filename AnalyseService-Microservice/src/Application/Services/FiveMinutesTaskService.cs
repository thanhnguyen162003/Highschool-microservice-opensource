using Application;
using Application.Common.Models.DaprModel.Enrollment;
using Application.Common.Models.DaprModel.Flashcard;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.StatisticModel;
using Dapr.Client;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

public class FiveMinutesTaskService(DaprClient daprClient, AnalyseDbContext dbContext) : BackgroundService
{
    private readonly DaprClient _daprClient = daprClient;
    private readonly AnalyseDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await AddUserFlashcardLearning();
                await AddUserLessonLearning();
                await AddUserRetention();
            }
            catch (Exception ex)
            {
                // Log the error (if logging is available)
                Console.WriteLine($"Error in scheduled task: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
    private async Task AddUserRetention()
    {
        try
        {
            var response = await _daprClient.InvokeMethodAsync<UserLoginCountResponseDapr>(
            HttpMethod.Get,
            "user-sidecar",
            $"api/v1/dapr/user-login-count"
            );
            var result = response.Retention
            .Select(x =>
            {
                if (!Guid.TryParse(x.UserId, out var userId))
                {
                    Console.WriteLine($"Invalid UserId: {x.UserId}");
                    return null; // Skip invalid UserId
                }

                if (!DateTime.TryParse(x.Date, out var loginDate))
                {
                    Console.WriteLine($"Invalid Date format: {x.Date}");
                    return null; // Skip invalid Date
                }
                if (!int.TryParse(x.RoleId, out var roleId))
                {
                    Console.WriteLine($"Invalid RoleId: {x.RoleId}");
                    return null; // Skip invalid Date
                }
                var check = userId;
                return new UserRetentionRequestModel
                {
                    UserId = userId,
                    LoginDate = loginDate,
                    RoleId = roleId
                };
            })
                .Where(x => x != null) // Remove null values caused by invalid data
                .ToList();
            var userIds = result.Select(x => x.UserId).ToList();

            var filter = Builders<UserRetentionModel>.Filter.In(ur => ur.UserId, userIds);

            var userRetention = await _dbContext.UserRetentionModel
                .Find(filter)
                .ToListAsync();

            // Step 2: Convert existing users to Dictionary for quick lookup
            var existingUserDict = userRetention.ToDictionary(ur => ur.UserId);

            // Step 3: Prepare bulk operations
            var bulkOps = new List<WriteModel<UserRetentionModel>>();

            foreach (var user in result)
            {
                if (existingUserDict.TryGetValue(user.UserId, out var existingUser))
                {
                    if (!existingUser.LoginDate.Any(d => d.Date == user.LoginDate.Date))
                    {
                        existingUser.LoginDate.Add(user.LoginDate);
                        existingUser.LoginDate = existingUser.LoginDate.OrderBy(d => d).Distinct().ToList();

                        int currentStreak = CalculateCurrentStreak(existingUser.LoginDate);
                        int maxStreak = CalculateMaxStreak(existingUser.LoginDate);

                        var update = Builders<UserRetentionModel>.Update
                            .AddToSet(ur => ur.LoginDate, user.LoginDate)
                            .Set(ur => ur.CurrentStreak, currentStreak)
                            .Set(ur => ur.MaxStreak, Math.Max(existingUser.MaxStreak, maxStreak));

                        var updateOneModel = new UpdateOneModel<UserRetentionModel>(
                            Builders<UserRetentionModel>.Filter.Eq(ur => ur.UserId, user.UserId),
                            update
                        );

                        bulkOps.Add(updateOneModel);
                    }
                }
                else
                {
                    var newUser = new UserRetentionModel
                    {
                        UserId = user.UserId,
                        LoginDate = new List<DateTime> { user.LoginDate },
                        RoleId = user.RoleId,
                        CurrentStreak = 1,
                        MaxStreak = 1
                    };

                    var insertOneModel = new InsertOneModel<UserRetentionModel>(newUser);
                    bulkOps.Add(insertOneModel);
                }
            }

            if (bulkOps.Count > 0)
            {
                await _dbContext.UserRetentionModel.BulkWriteAsync(bulkOps);
            }


        }
        catch (Exception e)
        {
            Console.WriteLine("Error " + e.Message);
        }
    }
    private async Task AddUserLessonLearning()
    {
        try
        {
            var response = await _daprClient.InvokeMethodAsync<EnrollmentResponseDapr>(
               HttpMethod.Get,
               "document-sidecar",
               $"api/v1/dapr/enrollment"
           );
            var today = DateTime.Today; // Get current date for comparison

            foreach (var enrollment in response.Enrollment)
            {
                var lessonLearnDates = enrollment.LessonLearnDate; // repeated string from proto
                if (lessonLearnDates.Count == 0) continue;

                var userIdGuid = Guid.Parse(enrollment.UserId);
                var filter = Builders<UserLessonLearningModel>.Filter.Eq(u => u.UserId, userIdGuid);
                var existingUser = await _dbContext.UserLessonLearningModel.Find(filter).FirstOrDefaultAsync();

                // Convert lessonLearnDates strings to DateTime and filter today's lessons
                var parsedDates = lessonLearnDates.Select(d => DateTime.Parse(d)).ToList();
                var todayLessonsCount = parsedDates.Count(d => d.Date == today);

                if (existingUser != null)
                {
                    // Calculate the difference based on today's count
                    int oldToday = existingUser.TodayLessonsLearned;
                    int diff = todayLessonsCount - oldToday;

                    if (diff != 0 || !existingUser.LearningDates.SequenceEqual(parsedDates))
                    {
                        var update = Builders<UserLessonLearningModel>.Update
                            .Set(u => u.TodayLessonsLearned, todayLessonsCount)
                            .Set(u => u.LearningDates, parsedDates)
                            .Inc(u => u.TotalLessonsLearned, diff);

                        await _dbContext.UserLessonLearningModel.UpdateOneAsync(filter, update);
                    }
                }
                else
                {
                    // Create new user record
                    var newUser = new UserLessonLearningModel
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = userIdGuid,
                        TodayLessonsLearned = todayLessonsCount,
                        TotalLessonsLearned = parsedDates.Count, // Total is all lessons, not just today's
                        LearningDates = parsedDates
                    };

                    await _dbContext.UserLessonLearningModel.InsertOneAsync(newUser);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    private async Task AddUserFlashcardLearning()
    {
        try
        {
            var list = await _daprClient.InvokeMethodAsync<UserFlashcardLearningResponseDapr>(
            HttpMethod.Get,
            "document-sidecar",
            $"api/v1/dapr/user-flashcard-learning"
            );
            var response = list.UserFlashcardLearning
            .GroupBy(x => new { x.FlashcardId, x.FlashcardContentId })
                .Select(group => new UserFlashcardLearningModel
                {
                    FlashcardId = Guid.Parse(group.Key.FlashcardId),
                    FlashcardContentId = Guid.Parse(group.Key.FlashcardContentId),
                    LearningDates = group
                        .SelectMany(x => x.LastReviewDateHistory.Select(DateTime.Parse))
                        .OrderBy(date => date)
                        .ToList(),
                    UserId = Guid.Parse(group.First().UserId),
                    TimeSpentHistory = group.SelectMany(x => x.TimeSpentHistory).ToList()

                })
                .ToList();

            var flashcardIds = response.Select(x => x.FlashcardId).ToList();
            var flashcardContentIds = response.Select(x => x.FlashcardContentId).ToList();

            var dbRecords = await _dbContext.UserFlashcardLearningModel
                .Find(x => flashcardIds.Contains(x.FlashcardId) && flashcardContentIds.Contains(x.FlashcardContentId))
                .ToListAsync();

            // Convert DB records into a dictionary for quick lookup
            var dbRecordsDict = dbRecords.ToDictionary(x => (x.FlashcardId, x.FlashcardContentId, x.UserId));

            // Lists for new inserts and updates
            List<UserFlashcardLearningModel> newRecords = new();
            List<ReplaceOneModel<UserFlashcardLearningModel>> updates = new();

            foreach (var record in response)
            {
                var key = (record.FlashcardId, record.FlashcardContentId, record.UserId);

                if (dbRecordsDict.TryGetValue(key, out var existingRecord))
                {
                    // Check if data is different
                    var test = !AreRecordsEqual(existingRecord, record);
                    if (!AreRecordsEqual(existingRecord, record))
                    {
                        record.Id = existingRecord.Id;
                        updates.Add(new ReplaceOneModel<UserFlashcardLearningModel>(
                            Builders<UserFlashcardLearningModel>.Filter.Eq(x => x.Id, existingRecord.Id),
                            record)
                        {
                            IsUpsert = false // Only update existing records
                        });
                    }
                }
                else
                {
                    // If record doesn't exist, add it as a new entry
                    newRecords.Add(record);
                }
            }

            // Batch update existing records
            if (updates.Any())
            {
                await _dbContext.UserFlashcardLearningModel.BulkWriteAsync(updates);
            }

            // Insert new records
            if (newRecords.Any())
            {
                await _dbContext.UserFlashcardLearningModel.InsertManyAsync(newRecords);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    public bool AreRecordsEqual(UserFlashcardLearningModel dbRecord, UserFlashcardLearningModel newRecord)
    {
        var test = dbRecord.FlashcardId == newRecord.FlashcardId;
        var test2 = dbRecord.FlashcardContentId == newRecord.FlashcardContentId;
        var test3 = dbRecord.UserId == newRecord.UserId;
        var test4 = dbRecord.LearningDates.Count == newRecord.LearningDates.Count;
        var test5 = dbRecord.LearningDates
    .Select(d => d.ToString("yyyy-MM-dd HH:mm:ss")) // Ignores milliseconds
    .SequenceEqual(newRecord.LearningDates.Select(d => d.ToString("yyyy-MM-dd HH:mm:ss")));

        return dbRecord.FlashcardId == newRecord.FlashcardId &&
               dbRecord.FlashcardContentId == newRecord.FlashcardContentId &&
               dbRecord.UserId == newRecord.UserId &&
               dbRecord.LearningDates.Count == newRecord.LearningDates.Count &&
               dbRecord.LearningDates
    .Select(d => d.ToString("yyyy-MM-dd HH:mm:ss")) // Ignores milliseconds
    .SequenceEqual(newRecord.LearningDates.Select(d => d.ToString("yyyy-MM-dd HH:mm:ss")))
               ;
    }
    private int CalculateCurrentStreak(List<DateTime> sortedLogins)
    {
        int streak = 0;
        DateTime today = DateTime.Today;
        var uniqueDates = sortedLogins.Select(x => x.Date).Distinct().OrderBy(x => x).ToList();
        for (int i = uniqueDates.Count - 1; i >= 0; i--)
        {
            if ((today.Date - uniqueDates[i].Date).TotalDays == streak)
            {
                streak++;
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    private int CalculateMaxStreak(List<DateTime> sortedLogins)
    {
        if (sortedLogins == null || sortedLogins.Count == 0) return 0;

        int maxStreak = 1, currentStreak = 1;
        var uniqueDates = sortedLogins.Select(x => x.Date).Distinct().OrderBy(x => x).ToList();
        for (int i = 1; i < uniqueDates.Count; i++)
        {
            if ((uniqueDates[i].Date - uniqueDates[i - 1].Date).TotalDays == 1)
            {
                currentStreak++;
            }
            else
            {
                maxStreak = Math.Max(maxStreak, currentStreak);
                currentStreak = 1; // Reset streak
            }
        }

        return Math.Max(maxStreak, currentStreak);
    }
}
