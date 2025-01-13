using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Data;
using Newtonsoft.Json;
using SharedProject.Models;
using MongoDB.Driver;
using Domain.Entities;
using SharedProject.Constaints;

namespace Application.Consumer;

public class RecentViewKafkaConsumer(
    IConfiguration configuration,
    ILogger<RecentViewKafkaConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBaseBatch<RecentViewModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.RecentViewCreated, ConsumerGroup.UserRecentViewGroup)
{
    protected override async Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AnalyseDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<RecentViewKafkaConsumer>>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var retryCount = 0;
        const int maxRetries = 2; // Max retry attempts
        const int delayBetweenRetriesMs = 2000; // Delay between retries in milliseconds

        var recentViewEntities = new List<RecentView>();
        foreach (var message in messages)
        {
            try
            {
                var recentViewModel = JsonConvert.DeserializeObject<RecentViewModel>(message);
                var newEntity = mapper.Map<RecentView>(recentViewModel);
                recentViewEntities.Add(newEntity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to deserialize message. Skipping this message.");
            }
        }

        while (retryCount < maxRetries)
        {
            try
            {
                foreach (var userIdGroup in recentViewEntities.GroupBy(e => e.UserId))
                {
                    var userId = userIdGroup.Key;
                    var userRecentViews = context.RecentViews
                        .Find(rv => rv.UserId == userId)
                        .SortByDescending(rv => rv.Time)
                        .ToList();

                    var newEntities = userIdGroup.ToList();

                    foreach (var newEntity in newEntities)
                    {
                        var existingEntry = userRecentViews.FirstOrDefault(rv => rv.IdDocument == newEntity.IdDocument);
                        if (existingEntry != null)
                        {
                            await context.RecentViews.DeleteOneAsync(rv => rv.Id == existingEntry.Id);
                            logger.LogInformation($"Removed old entry for UserId {userId}: DocumentId {existingEntry.IdDocument}");
                        }
                    }

                    // Remove the oldest entries if the count exceeds 9 after adding new entries
                    var totalEntries = userRecentViews.Count + newEntities.Count;
                    if (totalEntries > 10)
                    {
                        var excessCount = totalEntries - 10;
                        var oldestEntries = userRecentViews
                            .OrderBy(rv => rv.Time)
                            .Take(excessCount)
                            .ToList();

                        foreach (var oldestEntry in oldestEntries)
                        {
                            context.RecentViews.DeleteOne(rv => rv.Id == oldestEntry.Id);
                            logger.LogInformation($"Removed oldest entry for UserId {userId}: DocumentId {oldestEntry.IdDocument}");
                        }
                    }

                    // Insert new entries
                    if (newEntities.Any())
                    {
                        await context.RecentViews.InsertManyAsync(newEntities);
                        foreach (var newEntity in newEntities)
                        {
                            logger.LogInformation($"Saved recent view for UserId {newEntity.UserId}: DocumentId {newEntity.IdDocument}");
                        }
                    }
                }

                // Exit retry loop after success
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                logger.LogError(ex, $"Attempt {retryCount} failed while processing batch. Retrying...");

                if (retryCount >= maxRetries)
                {
                    logger.LogError(ex, "Maximum retries reached. Batch processing failed.");
                }
                else
                {
                    await Task.Delay(delayBetweenRetriesMs);
                }
            }
        }
    }
}
