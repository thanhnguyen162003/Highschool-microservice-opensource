using Application.Features.RoadmapUser.CreateRoadmapUser;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Constants;
using Newtonsoft.Json;
using SharedProject.Models;
using System.Net;
using Application.Common.Kafka;
using Application.Constants;

namespace Application.Services.EventHandler.RetryConsumer
{
    public class ConsumerUserRoadmapRetryService(
        IConfiguration configuration,
        ILogger<ConsumerUserRoadmapRetryService> logger,
        IServiceProvider serviceProvider)
        : KafkaConsumerBaseBatch<RoadmapUserKafkaMessageModel>(configuration, logger, serviceProvider,
            TopicConstant.UserRoadmapGenCreatedRetry, "user-roadmap-retry-batch-group")
    {

        protected override async Task ProcessBatch(IEnumerable<string> messages, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var logger = scopedProvider.GetRequiredService<ILogger<ConsumerUserRoadmapService>>();
            var sender = scopedProvider.GetRequiredService<ISender>();

            const int maxRetries = 2;
            var retryMessages = new Dictionary<RoadmapUserKafkaMessageModel, int>();
            var successCount = 0;

            foreach (var message in messages)
            {
                try
                {
                    var roadmapUserKafkaMessageModel = JsonConvert.DeserializeObject<RoadmapUserKafkaMessageModel>(message);
                    if (roadmapUserKafkaMessageModel == null)
                    {
                        logger.LogWarning(DataConstaints.UserServiceLogFirst + "Skipping invalid or null message.");
                        continue;
                    }

                    var command = new CreateRoadmapUserCommand
                    {
                        RoadmapUserKafkaMessageModel = roadmapUserKafkaMessageModel
                    };

                    var result = await sender.Send(command);

                    if (result.Status != HttpStatusCode.OK)
                    {
                        logger.LogError(DataConstaints.UserServiceLogFirst + $"Error processing message for UserId: {roadmapUserKafkaMessageModel.UserId}. Adding to retry list.");
                        retryMessages[roadmapUserKafkaMessageModel] = 1;
                    }
                    else
                    {
                        successCount++;
                        logger.LogInformation(DataConstaints.UserServiceLogFirst + $"Successfully processed message for UserId: {roadmapUserKafkaMessageModel.UserId}.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,DataConstaints.UserServiceLogFirst + "Error processing message. Adding to retry list.");
                    var roadmapUserKafkaMessageModel = JsonConvert.DeserializeObject<RoadmapUserKafkaMessageModel>(message);
                    if (roadmapUserKafkaMessageModel != null)
                    {
                        retryMessages[roadmapUserKafkaMessageModel] = 1;
                    }
                }
            }

            // Retry logic for failed messages
            while (retryMessages.Any())
            {
                var retryBatch = retryMessages.Keys.ToList();
                retryMessages.Clear(); // Clear for this round

                foreach (var retryMessage in retryBatch)
                {
                    try
                    {
                        var command = new CreateRoadmapUserCommand
                        {
                            RoadmapUserKafkaMessageModel = retryMessage
                        };

                        var result = await sender.Send(command);

                        if (result.Status != HttpStatusCode.OK)
                        {
                            throw new Exception(DataConstaints.UserServiceLogFirst + $"Failed to process message for UserId: {retryMessage.UserId}");
                        }

                        successCount++;
                        logger.LogInformation(DataConstaints.UserServiceLogFirst + $"Successfully retried message for UserId: {retryMessage.UserId}.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex,DataConstaints.UserServiceLogFirst + $"Retry failed for UserId: {retryMessage.UserId}");
                        
                        // Increment retry counter and re-add to retryMessages if not exceeding maxRetries
                        if (!retryMessages.ContainsKey(retryMessage))
                        {
                            retryMessages[retryMessage] = 1;
                        }

                        retryMessages[retryMessage]++;

                        if (retryMessages[retryMessage] > maxRetries)
                        {
                            //go to dead letter queue
                            logger.LogError(DataConstaints.UserServiceLogFirst + $"Max retries exceeded for UserId: {retryMessage.UserId}. Sending to deadletter topic.");
                            // await producer.ProduceObjectWithKeyAsync(
                            //     TopicConstant.UserRoadmapGenCreatedRetry,
                            //     retryMessage.UserId.ToString(),
                            //     retryMessage);
                        }
                    }
                }
            }

            logger.LogInformation(DataConstaints.UserServiceLogFirst + $"Batch processing complete. {successCount} messages processed successfully.");
        }
    }
}
