using Application.Common.Kafka;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;
using Newtonsoft.Json;
using SharedProject.Constaints;
using SharedProject.Models;

namespace Application.Consumer;

public class RecentViewDeleteConsumer(
    IConfiguration configuration,
    ILogger<RecentViewDeleteConsumer> logger,
    IServiceProvider serviceProvider)
    : UniqueKafkaConsumerBase<RecentViewModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.RecentViewDeleted, ConsumerGroup.UserRecentViewGroup)
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AnalyseDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<RecentViewDeleteConsumer>>();

        try
        {
            var recentViewModel = JsonConvert.DeserializeObject<RecentViewDeleteModel>(message);
            if (recentViewModel == null)
            {
                logger.LogWarning("Received an empty or invalid message.");
                return;
            }

            var filter = Builders<RecentView>.Filter.And(
                Builders<RecentView>.Filter.Eq(rv => rv.IdDocument, recentViewModel.IdDocument),
                Builders<RecentView>.Filter.Eq(rv => rv.TypeDocument, recentViewModel.TypeDocument)
            );

            var deleteResult = await context.RecentViews.DeleteManyAsync(filter);
            if (deleteResult.DeletedCount > 0)
            {
                logger.LogInformation($"Deleted {deleteResult.DeletedCount} recent view(s) for IdDocument {recentViewModel.IdDocument} and TypeDocument {recentViewModel.TypeDocument}");
            }
            else
            {
                logger.LogInformation($"No recent views found to delete for IdDocument {recentViewModel.IdDocument} and TypeDocument {recentViewModel.TypeDocument}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message.");
        }
    }
}
