using System.Text.Json;
using Application.Common.Interfaces;
using Application.Constants;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.Enums;
using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Features.NotificationFeature
{
    public class FlashcardNotificationConsumer(
        IConfiguration configuration,
        ILogger<FlashcardNotificationConsumer> logger,
        IServiceProvider serviceProvider) : KafkaConsumerBase<FlashcardDueNotificationModel>(configuration, logger, serviceProvider,
               TopicKafkaConstaints.FlashcardDueNotification, "notification-group")
    {
        protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<FlashcardNotificationConsumer>>();
            var novuService = serviceProvider.GetRequiredService<INovuService>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            FlashcardDueNotificationModel? dataModel = JsonSerializer.Deserialize<FlashcardDueNotificationModel>(message, options);

            if (dataModel == null)
            {
                logger.LogError("Failed to deserialize flashcard notification model");
                return;
            }

            logger.LogInformation("Processing flashcard notification for user {UserId}, flashcard {FlashcardName} with {Count} due items",
                dataModel.UserId, dataModel.FlashcardName, dataModel.DueContentCount);

            try
            {
                var notificationPayload = new
                {
                    title = "Bạn có flashcard cần review",
                    content = dataModel.FlashcardName + $" có {dataModel.DueContentCount} flashcard để review.",
                    reviewUrl = $"/{dataModel.FlashcardSlug}",
                    createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    type = NotificationConstaints.FlashcardNotification
                };

                var novuModel = new NovuTriggerNotificationModel
                {
                    WorkflowId = "flashcard-notification",
                    TargetId = dataModel.UserId.ToString(),
                    Payload = notificationPayload,
                    NotificationTriggerType = NotificationTriggerType.Users
                };

                var result = await novuService.TriggerNotification(novuModel);

                if (result)
                {
                    logger.LogInformation("Novu notification sent to user {UserId} for flashcard {FlashcardName}",
                        dataModel.UserId, dataModel.FlashcardName);
                }
                else
                {
                    logger.LogError("Failed to send Novu notification to user {UserId} for flashcard {FlashcardName}",
                        dataModel.UserId, dataModel.FlashcardName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing flashcard notification for user {UserId}", dataModel.UserId);
            }
        }
    }

}