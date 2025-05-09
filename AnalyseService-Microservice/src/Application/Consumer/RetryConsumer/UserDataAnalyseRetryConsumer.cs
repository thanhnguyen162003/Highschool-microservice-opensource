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

namespace Application.Consumer.RetryConsumer;

public class UserDataAnalyseRetryConsumer(IConfiguration configuration, ILogger<UserDataAnalyseRetryConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<UserDataAnalyseModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.RecommendOnboardingRetry, ConsumerGroup.UserDataAnalyzeGroup)
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        // var _redis = serviceProvider.GetRequiredService<IOrdinaryDistributedCache>();
        var context = serviceProvider.GetRequiredService<AnalyseDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<UserDataAnalyseRetryConsumer>>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();
        var userModel = JsonConvert.DeserializeObject<UserDataAnalyseModel>(message);
        UserAnalyseEntity entity = mapper.Map<UserAnalyseEntity>(userModel);
        try
        {
            // Check if an entity with the same UserId already exists
            var existingEntity = await context.UserAnalyseEntity
                .Find(e => e.UserId.Equals(entity.UserId))
                .FirstOrDefaultAsync();
            
            if (existingEntity is null && userModel!.Address is not null && userModel.TypeExam is not null)
            {
                string mongoId = ObjectId.GenerateNewId().ToString();
                RecommendedData recommendedData = new RecommendedData()
                {
                    Id = mongoId,
                    UserId = entity.UserId,
                    SubjectIds = entity.Subjects,
                    Grade = entity.Grade,
                    TypeExam = entity.TypeExam
                };
                UserAnalyseEntity userDataEntity = new UserAnalyseEntity()
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
                await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataRecommended, entity.UserId.ToString(), recommendedData);
                await context.UserAnalyseEntity.InsertOneAsync(userDataEntity);
            }
            if (existingEntity is not null && userModel!.Address is not null && userModel.TypeExam is not null)
            {
                logger.LogInformation($"User with UserId {entity.UserId} already exists. Performing update...");
                existingEntity.Address = entity.Address;
                existingEntity.Grade = entity.Grade;
                existingEntity.UserId = entity.UserId;
                existingEntity.SchoolName = entity.SchoolName;
                existingEntity.Major = entity.Major;
                existingEntity.TypeExam = entity.TypeExam;
                existingEntity.Subjects = entity.Subjects;
                
                RecommendedData recommendedData = new RecommendedData()
                {
                    UserId = entity.UserId,
                    SubjectIds = entity.Subjects,
                    Grade = entity.Grade,
                    TypeExam = entity.TypeExam,
                    Id = existingEntity.Id
                };
                await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DataRecommended, entity.UserId.ToString(), recommendedData);
                await context.UserAnalyseEntity.ReplaceOneAsync(
                    e => e.UserId == existingEntity.UserId,
                    existingEntity
                );

                logger.LogInformation($"UserDataAnalyse entity updated: {existingEntity.Id}");
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "this consumer have retry manytime in data recommend it will go to dead letter");
            await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.RecommendOnboardingDeadLetter, userModel.UserId.ToString(), userModel);
            logger.LogError(ex, "An error occurred while processing cache operations for key {ex}.", ex.Message);
        }
    }
}
