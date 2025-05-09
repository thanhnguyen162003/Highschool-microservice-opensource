using System.Net;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.SearchModel;
using Application.Common.Models.StatisticModel;
using Dapr.Client;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.StatisticFeature.Commands;

public record AddUserRetentionCommand : IRequest<ResponseModel>
{
}

public class AddUserRetentionCommanddHandler(
    AnalyseDbContext dbContext,
    DaprClient daprClient,
    ILogger<AddUserActivityCommandHandler> logger)
    : IRequestHandler<AddUserRetentionCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(AddUserRetentionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await daprClient.InvokeMethodAsync<UserLoginCountResponseDapr>(
            HttpMethod.Get,
            "user-sidecar",
            $"api/v1/dapr/user-login-count");
            logger.LogInformation($"Response Dapr: {response.Retention.Count}");
            var result = response.Retention
                .Select(x =>
                {
                    if (!Guid.TryParse(x.UserId, out var userId))
                    {
                        logger.LogError($"Invalid UserId: {x.UserId}");
                        return null; // Skip invalid UserId
                    }

                    if (!DateTime.TryParse(x.Date, out var loginDate))
                    {
                        logger.LogError($"Invalid Date format: {x.Date}");
                        return null; // Skip invalid Date
                    }
                    if (!int.TryParse(x.RoleId, out var roleId))
                    {
                        logger.LogError($"Invalid RoleId: {x.RoleId}");
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

            var userRetention = await dbContext.UserRetentionModel
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
                await dbContext.UserRetentionModel.BulkWriteAsync(bulkOps);
            }

            return new ResponseModel(HttpStatusCode.Created, "thành công", bulkOps);
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            return new ResponseModel(HttpStatusCode.BadRequest, e.Message);
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
