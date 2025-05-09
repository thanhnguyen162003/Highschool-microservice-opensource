using System.Text.Json;
using Application.Common.Interfaces;
using Application.Constants;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.Enums;
using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Features.NotificationFeature;

public class SystemNotificationConsumer(IConfiguration configuration, ILogger<SystemNotificationConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<NotificationSystemModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.NotificationSystemCreated, "notification-group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<SystemNotificationConsumer>>();
        var novuService = serviceProvider.GetRequiredService<INovuService>();

        NotificationSystemModel? dataModel = JsonSerializer.Deserialize<NotificationSystemModel>(message);
        if (dataModel == null)
        {
            logger.LogError("Failed to deserialize notification system model");
            return;
        }

        var notificationPayload = new
        {
            title = dataModel.Title,
            content = dataModel.Content,
            createdAt = dataModel.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            type = NotificationConstaints.SystemNotification
        };

        var novuModel = new NovuTriggerNotificationModel
        {
            WorkflowId = "system-notification",
            Payload = notificationPayload,
            NotificationTriggerType = NotificationTriggerType.SystemWide
        };

        var result = await novuService.TriggerNotification(novuModel);
        if (result)
        {
            logger.LogInformation($"Novu system notification broadcast sent: {dataModel.Title}");
        }
        else
        {
            logger.LogError($"Failed to send Novu system notification broadcast");
        }
    }
}