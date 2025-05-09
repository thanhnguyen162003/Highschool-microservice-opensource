using System.Text.Json;
using Application.Common.Kafka;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedProject.ConsumeModel;

namespace Application.Consumers;

public class DataRecommendedConsumer(
    IConfiguration configuration,
    ILogger<DataRecommendedConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBaseBatch<RecommendedDataModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.DataRecommended, "data-recommended-consumer-group")
{
    protected override async Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<DocumentDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<DataRecommendedConsumer>>();
        var newEntries = new List<RecommendedData>();
        var updatedEntries = new List<RecommendedData>();
        var retryMessages = new List<string>();

        foreach (var message in messages)
        {
            try
            {
                var recommendedDataModel = JsonSerializer.Deserialize<RecommendedDataModel>(message);
                if (recommendedDataModel is null)
                {
                    logger.LogWarning("Skipping invalid or null message.");
                    continue;
                }

                List<Guid> subjectIds = recommendedDataModel.SubjectIds;
                
                var existingData = await dbContext.RecommendedDatas
                    .AsNoTracking()
                    .Where(x => x.UserId.Equals(recommendedDataModel.UserId))
                    .FirstOrDefaultAsync();

                if (existingData is not null)
                {
                    var updatedEntry = new RecommendedData
                    {
                        Id = existingData.Id,
                        SubjectIds = JsonSerializer.Serialize(subjectIds),
                        UserId = recommendedDataModel.UserId,
                        Grade = recommendedDataModel.Grade,
                        ObjectId = recommendedDataModel.Id,
                        TypeExam = recommendedDataModel.TypeExam,
                    };
                    updatedEntries.Add(updatedEntry);
                }
                else
                {
                    var newEntry = new RecommendedData
                    {
                        Id = new UuidV7().Value,
                        SubjectIds = JsonSerializer.Serialize(subjectIds),
                        UserId = recommendedDataModel.UserId,
                        Grade = recommendedDataModel.Grade,
                        ObjectId = recommendedDataModel.Id,
                        TypeExam = recommendedDataModel.TypeExam,
                    };
                    newEntries.Add(newEntry);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message. Adding to retry list.");
                retryMessages.Add(message);
            }
        }

        try
        {
            // Bulk insert new entries
            if (newEntries.Any())
            {
                await dbContext.RecommendedDatas.AddRangeAsync(newEntries);
            }

            // Bulk update existing entries
            if (updatedEntries.Any())
            {
                dbContext.RecommendedDatas.UpdateRange(updatedEntries);
            }

            var result = await dbContext.SaveChangesAsync();
            logger.LogInformation($"Processed batch: {newEntries.Count} new, {updatedEntries.Count} updated. {result} changes saved.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving batch to database. Retrying failed messages.");
            retryMessages.AddRange(messages); // Retry the entire batch in case of DB save failure
        }

        // Retry handling
        if (retryMessages.Any())
        {
            logger.LogWarning($"Retrying {retryMessages.Count} failed messages.");
            foreach (var retryMessage in retryMessages)
            {
                // Can use a retry mechanism or produce these to a retry topic
                logger.LogWarning($"Retry message: {retryMessage}");
            }
        }
    }
}
