using System.Text.Json;
using Application.Common.Interfaces;
using Application.Constants;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.Enums;
using SharedProjects.ConsumeModel.NovuModel;
namespace Application.Features.NotificationFeature;

public class NotificationDocumentConsumer(IConfiguration configuration, ILogger<NotificationDocumentConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<NotificationDocumentModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.NotificationDocumentCreated, "notification-group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<NotificationDocumentConsumer>>();
        var novuService = serviceProvider.GetRequiredService<INovuService>();

        NotificationDocumentModel? dataModel = JsonSerializer.Deserialize<NotificationDocumentModel>(message);
        if (dataModel == null)
        {
            logger.LogError("Failed to deserialize notification document model");
            return;
        }

        var notificationPayload = new
        {
            title = dataModel.Title,
            content = dataModel.Content,
            createdAt = dataModel.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            type = NotificationConstaints.DocumentNotification
        };
            // Send notification to a specific user via Novu
            var novuModel = new NovuTriggerNotificationModel
            {
                WorkflowId = "document-notification", 
                TargetId = dataModel.UserId,
                Payload = notificationPayload,
                NotificationTriggerType = NotificationTriggerType.Users
            };

            var result = await novuService.TriggerNotification(novuModel);
            if (result)
            {
                logger.LogInformation($"Novu notification sent to user {dataModel.UserId}: {dataModel.Title}");
            }
            else
            {
                logger.LogError($"Failed to send Novu notification to user {dataModel.UserId}");
            }
    }
}