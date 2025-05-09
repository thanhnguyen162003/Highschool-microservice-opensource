using System.Text.Json;
using Application.Common.Interfaces;
using Application.Constants;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.Enums;
using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Features.NotificationFeature;

public class NotificationUserConsumer(IConfiguration configuration, ILogger<NotificationUserConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<NotificationUserModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.NotificationUserCreated, "notification-group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<NotificationUserConsumer>>();
        var novuService = serviceProvider.GetRequiredService<INovuService>();

        NotificationUserModel? dataModel = JsonSerializer.Deserialize<NotificationUserModel>(message);
        if (dataModel == null)
        {
            logger.LogError("Failed to deserialize notification user model");
            return;
        }

        var notificationPayload = new
        {
            title = dataModel.Title,
            content = dataModel.Content,
            createdAt = dataModel.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            type = NotificationConstaints.UserNotification
        };
            var novuModel = new NovuTriggerNotificationModel
            {
                WorkflowId = "user-notification",
                TargetId = dataModel.UserId.ToString(),
                Payload = notificationPayload,
                NotificationTriggerType = NotificationTriggerType.Users
            };

            var result = await novuService.TriggerNotification(novuModel);
            if (result)
            {
                logger.LogInformation($"Novu notification sent to user {novuModel.TargetId}: {dataModel.Title}");
            }
            else
            {
                logger.LogError($"Failed to send Novu notification to user {novuModel.TargetId}");
            }
        
    }
}