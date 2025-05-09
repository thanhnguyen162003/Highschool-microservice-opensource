using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Application.KafkaMessageModel;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SharedProject.Constaints;
using SharedProject.Models;

namespace Application.Consumer.RetryConsumer;

public class UserRoadmapGenRetryConsumer(IConfiguration configuration, ILogger<UserRoadmapGenRetryConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<UserDataAnalyseModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.RecommendOnboardingRetryRoadmapGen, ConsumerGroup.UserDataAnalyzeRoadmapGroup)
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        // var _redis = serviceProvider.GetRequiredService<IOrdinaryDistributedCache>();
        var context = serviceProvider.GetRequiredService<AnalyseDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<UserRoadmapGenRetryConsumer>>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();
        var userModel = JsonConvert.DeserializeObject<UserDataAnalyseModel>(message);
        UserAnalyseEntity entity = mapper.Map<UserAnalyseEntity>(userModel);
        try
        {
            //base on user subjectIds, find all the roadmapSubjectIds that most match with user subjectIds
            //then, send back to user 1
            string mongoId = ObjectId.GenerateNewId().ToString();
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
                        //intersect 2 list to get the number of matching subjectIds
                        MatchingSubjects = roadmap.RoadmapSubjectIds.Intersect(subjectIds).Count(),
                        MatchingTypeExam = roadmap.TypeExam.Intersect(
                        entity.TypeExam?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>()).Count(),
                    })
                    .OrderByDescending(x => x.MatchingSubjects)
                    .ThenByDescending(x => x.MatchingTypeExam)
                    .Take(4)
                    .Select(x => x.Roadmap)
                    .ToList();
                //send back to user 1
                if (matchingRoadmaps.Count > 0)
                {
                    var random = new Random();
                    var selectedRoadmap = matchingRoadmaps[random.Next(matchingRoadmaps.Count)];
                    RoadmapUserKafkaMessageModel messageModel = new RoadmapUserKafkaMessageModel(){
                        RoadmapId = selectedRoadmap.Id,
                        RoadmapName = selectedRoadmap.RoadmapName,
                        RoadmapSubjectIds = selectedRoadmap.RoadmapSubjectIds,
                        RoadmapDescription = selectedRoadmap.RoadmapDescription,
                        TypeExam = selectedRoadmap.TypeExam,
                        ContentJson = selectedRoadmap.ContentJson,
                        RoadmapDocumentIds = selectedRoadmap.RoadmapDocumentIds,
                        UserId = entity.UserId
                    };
                    await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.UserRoadmapGenCreated, entity.UserId.ToString(), messageModel);
                }
                else
                {
                    logger.LogWarning($"No matching roadmaps found for user {entity.UserId}.");
                    // Handle the case where no matching roadmaps are found
                }
            }
            
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "this consumer have retry manytime in data recommend it will go to dead letter");
            await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecommendOnboardingDeadLetterRoadmapGen,
                userModel.UserId.ToString(), userModel);
            logger.LogError(ex, "An error occurred while processing cache operations for key {ex}.", ex.Message);
        }
    }
}
