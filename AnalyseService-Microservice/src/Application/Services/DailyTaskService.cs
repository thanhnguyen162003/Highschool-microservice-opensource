using Application;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.StatisticModel;
using Dapr.Client;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;

public class DailyTaskService(AnalyseDbContext dbContext, DaprClient daprClient) : BackgroundService
{
    private TimeSpan _targetTime = new TimeSpan(23, 59, 50);
    private readonly DaprClient _daprClient = daprClient;
    private readonly AnalyseDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var utcPlus7Zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // UTC+7
            var now = TimeZoneInfo.ConvertTime(DateTime.Now, utcPlus7Zone);
            var nextRun = now.Date + _targetTime;
            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;
            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                await AddUserActivity();
                //await AddUserRetention();
            }
        }
    }

    private async Task AddUserActivity()
    {
        
        var request = new UserCountRequest
        {
            Amount = 0,
            IsCount = false,
            Type = "Day"
        };
        var response = await _daprClient.InvokeMethodAsync<UserCountResponseDapr>(
                       HttpMethod.Get,
                       "user-sidecar",
                       $"api/v1/dapr/user-count?Type={request.Type}&Amount={request.Amount}&IsCount={request.IsCount}"
                   );


        var result = response.Activities.Select(x => new UserActivityModel
        {
            Date = DateTime.Parse(x.Date),
            Moderators = x.Moderators,
            Students = x.Students,
            Teachers = x.Teachers

        }).ToList();
        await _dbContext.UserActivityModel.InsertManyAsync(result);
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

