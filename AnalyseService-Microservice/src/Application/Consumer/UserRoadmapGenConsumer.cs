using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;
using Newtonsoft.Json;
using SharedProject.Constaints;
using SharedProject.Models;

namespace Application.Consumer;

public class UserRoadmapGenConsumer(
    IConfiguration configuration,
    ILogger<UserRoadmapGenConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBaseBatch<UserDataAnalyseModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.RecommendOnboarding, ConsumerGroup.UserDataAnalyzeRoadmapGroup)
{
    protected override async Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AnalyseDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<UserRoadmapGenConsumer>>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var producer = serviceProvider.GetRequiredService<IProducerBatchService>(); // Use the batch producer

        foreach (var message in messages)
        {
            try
            {
                var userModel = JsonConvert.DeserializeObject<UserDataAnalyseModel>(message);
                if (userModel is null)
                {
                    logger.LogWarning("Deserialized message is null. Skipping...");
                    continue;
                }

                UserAnalyseEntity entity = mapper.Map<UserAnalyseEntity>(userModel);
                List<Guid> subjectIds = entity.Subjects;

                if (subjectIds.Count >= 3)
                {
                    var roadmaps = await context.Roadmap
                        .Find(_ => true)
                        .ToListAsync();

                    var matchingRoadmaps = roadmaps
                        .Select(roadmap => new
                        {
                            Roadmap = roadmap,
                            MatchingSubjects = roadmap.RoadmapSubjectIds.Intersect(subjectIds).Count(),
                            MatchingTypeExam = roadmap.TypeExam.Intersect(
                                entity.TypeExam?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                ?? Array.Empty<string>()).Count(),
                        })
                        .OrderByDescending(x => x.MatchingSubjects)
                        .ThenByDescending(x => x.MatchingTypeExam)
                        .Take(4)
                        .Select(x => x.Roadmap)
                        .ToList();

                    if (matchingRoadmaps.Count > 0)
                    {
                        var random = new Random();
                        var selectedRoadmap = matchingRoadmaps[random.Next(matchingRoadmaps.Count)];

                        RoadmapUserKafkaMessageModel messageModel = new()
                        {
                            RoadmapId = selectedRoadmap.Id,
                            RoadmapName = selectedRoadmap.RoadmapName,
                            RoadmapSubjectIds = selectedRoadmap.RoadmapSubjectIds,
                            RoadmapDescription = selectedRoadmap.RoadmapDescription,
                            TypeExam = selectedRoadmap.TypeExam,
                            ContentJson = selectedRoadmap.ContentJson,
                            RoadmapDocumentIds = selectedRoadmap.RoadmapDocumentIds,
                            UserId = entity.UserId
                        };

                        producer.QueueMessage(TopicKafkaConstaints.UserRoadmapGenCreated, entity.UserId.ToString(), messageModel);
                    }
                    else
                    {
                        logger.LogWarning($"No matching roadmaps found for user {entity.UserId}.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process message. Adding to retry messages...");
                var userModel = JsonConvert.DeserializeObject<UserDataAnalyseModel>(message);
                if (userModel != null)
                {
                    producer.QueueMessage(TopicKafkaConstaints.RecommendOnboardingRetryRoadmapGen, userModel.UserId.ToString(), userModel);
                }
            }
        }

        logger.LogInformation("All messages queued for batch production.");
    }
}
