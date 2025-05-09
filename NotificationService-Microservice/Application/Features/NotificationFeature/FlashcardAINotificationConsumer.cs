using System;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Constants;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.Enums;
using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Features.NotificationFeature;

public class FlashcardAINotificationConsumer(
    IConfiguration configuration,
    ILogger<FlashcardAINotificationConsumer> logger,
    IServiceProvider serviceProvider) : KafkaConsumerBase<NotificationFlashcardAIGenModel>(configuration, logger, serviceProvider,
           TopicKafkaConstaints.NotificationFlashcardAIGen, "notification-group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<FlashcardAINotificationConsumer>>();
            var novuService = serviceProvider.GetRequiredService<INovuService>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            NotificationFlashcardAIGenModel? dataModel = JsonSerializer.Deserialize<NotificationFlashcardAIGenModel>(message, options);

            if (dataModel == null)
            {
                logger.LogError("Failed to deserialize flashcard notification model");
                return;
            }
            try
            {
                var notificationPayload = new
                {
                    title = dataModel.Title,
                    content = dataModel.Content,
                    flashcardId = dataModel.FlashcardId.ToString(),
                    createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    type = NotificationConstaints.FlashcardAIGenNotification
                };

                var novuModel = new NovuTriggerNotificationModel
                {
                    WorkflowId = "flashcard-ai-gen-notification",
                    TargetId = dataModel.UserId.ToString(),
                    Payload = notificationPayload,
                    NotificationTriggerType = NotificationTriggerType.Users
                };

                var result = await novuService.TriggerNotification(novuModel);

                if (result)
                {
                    logger.LogInformation("Novu notification sent to user {UserId} for flashcard {FlashcardName}",
                        dataModel.UserId, dataModel.FlashcardId);
                }
                else
                {
                    logger.LogError("Failed to send Novu notification to user {UserId} for flashcard {FlashcardId}",
                        dataModel.UserId, dataModel.FlashcardId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing flashcard notification for user {UserId}", dataModel.UserId);
            }
        }
}