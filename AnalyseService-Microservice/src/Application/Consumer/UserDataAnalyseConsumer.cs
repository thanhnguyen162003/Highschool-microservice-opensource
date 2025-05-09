using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SharedProject.Constaints;
using SharedProject.Models;

namespace Application.Consumer;

public class UserDataAnalyseConsumer(
    IConfiguration configuration,
    ILogger<UserDataAnalyseConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBaseBatch<UserDataAnalyseModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.RecommendOnboarding, ConsumerGroup.UserDataAnalyzeGroup)
{
    protected override async Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AnalyseDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<UserDataAnalyseConsumer>>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();

        var userEntitiesToInsert = new List<UserAnalyseEntity>();
        var userEntitiesToUpdate = new List<UserAnalyseEntity>();
        var recommendedDataToProduce = new List<(string Key, RecommendedData Data)>();
        var retryMessages = new List<(string Key, UserDataAnalyseModel Message)>();

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

                var existingEntity = await context.UserAnalyseEntity
                    .Find(e => e.UserId.Equals(entity.UserId))
                    .FirstOrDefaultAsync();

                if (existingEntity is null && userModel.Address is not null && userModel.TypeExam is not null)
                {
                    string mongoId = ObjectId.GenerateNewId().ToString();
                    RecommendedData recommendedData = new()
                    {
                        Id = mongoId,
                        UserId = entity.UserId,
                        SubjectIds = entity.Subjects,
                        Grade = entity.Grade,
                        TypeExam = entity.TypeExam
                    };
                    UserAnalyseEntity newEntity = new()
                    {
                        Id = mongoId,
                        Address = entity.Address,
                        Grade = entity.Grade,
                        UserId = entity.UserId,
                        SchoolName = entity.SchoolName,
                        Major = entity.Major,
                        TypeExam = entity.TypeExam,
                        Subjects = entity.Subjects
                    };

                    userEntitiesToInsert.Add(newEntity);
                    recommendedDataToProduce.Add((entity.UserId.ToString(), recommendedData));
                }
                else if (existingEntity is not null && userModel.Address is not null && userModel.TypeExam is not null)
                {
                    logger.LogInformation($"User with UserId {entity.UserId} already exists. Adding to update list...");

                    existingEntity.Address = entity.Address;
                    existingEntity.Grade = entity.Grade;
                    existingEntity.UserId = entity.UserId;
                    existingEntity.SchoolName = entity.SchoolName;
                    existingEntity.Major = entity.Major;
                    existingEntity.TypeExam = entity.TypeExam;
                    existingEntity.Subjects = entity.Subjects;

                    RecommendedData recommendedData = new()
                    {
                        UserId = entity.UserId,
                        SubjectIds = entity.Subjects,
                        Grade = entity.Grade,
                        TypeExam = entity.TypeExam,
                        Id = existingEntity.Id
                    };

                    userEntitiesToUpdate.Add(existingEntity);
                    recommendedDataToProduce.Add((entity.UserId.ToString(), recommendedData));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process message. Adding to retry messages...");
                retryMessages.Add((message, JsonConvert.DeserializeObject<UserDataAnalyseModel>(message)!));
            }
        }

        try
        {
            if (userEntitiesToInsert.Any())
            {
                await context.UserAnalyseEntity.InsertManyAsync(userEntitiesToInsert);
                logger.LogInformation($"Inserted {userEntitiesToInsert.Count} user data entities.");
            }

            foreach (var entity in userEntitiesToUpdate)
            {
                await context.UserAnalyseEntity.ReplaceOneAsync(
                    e => e.UserId == entity.UserId,
                    entity);
            }
            logger.LogInformation($"Updated {userEntitiesToUpdate.Count} user data entities.");

            foreach (var (key, data) in recommendedDataToProduce)
            {
                await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataRecommended, key, data);
            }
            logger.LogInformation($"Produced {recommendedDataToProduce.Count} recommended data messages.");

            foreach (var (key, message) in retryMessages)
            {
                await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecommendOnboardingRetry, key, message);
            }
            logger.LogInformation($"Retried {retryMessages.Count} messages.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed during batch operations.");
        }
    }
}
