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
    : UniqueKafkaConsumerBase<RecentViewModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.RecentViewCreated, ConsumerGroup.UserRecentViewGroup)
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AnalyseDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<RecentViewKafkaConsumer>>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        try
        {
            var recentViewModel = JsonConvert.DeserializeObject<RecentViewModel>(message);
            if (recentViewModel == null)
            {
                logger.LogWarning("Received an empty or invalid message.");
                return;
            }

            var newEntity = mapper.Map<RecentView>(recentViewModel);
            var userId = newEntity.UserId;

            var userRecentViews = context.RecentViews
                .Find(rv => rv.UserId == userId)
                .SortByDescending(rv => rv.Time)
                .ToList();

            // Check if document already exists and remove it
            var existingEntry = userRecentViews.FirstOrDefault(rv => rv.IdDocument == newEntity.IdDocument);
            if (existingEntry != null)
            {
                await context.RecentViews.DeleteOneAsync(rv => rv.Id == existingEntry.Id);
                logger.LogInformation($"Removed old entry for UserId {userId}: DocumentId {existingEntry.IdDocument}");
            }

            if (userRecentViews.Count >= 10)
            {
                var oldestEntry = userRecentViews.Last();
                await context.RecentViews.DeleteOneAsync(rv => rv.Id == oldestEntry.Id);
                logger.LogInformation($"Removed oldest entry for UserId {userId}: DocumentId {oldestEntry.IdDocument}");
            }

            // Insert new recent view entry
            await context.RecentViews.InsertOneAsync(newEntity);
            logger.LogInformation($"Saved recent view for UserId {userId}: DocumentId {newEntity.IdDocument}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message.");
        }
    }
}
