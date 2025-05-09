using System.Text.Json;
using Application.Common.Interfaces;
using Application.Constants;
using SharedProjects.ConsumeModel;
using SharedProjects.ConsumeModel.Enums;
using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Features.NotificationFeature;

public class NewsNotificationConsumer(IConfiguration configuration, ILogger<NewsNotificationConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<NotificationNewsModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.NotificationNewsCreated, "notification-group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<NewsNotificationConsumer>>();
        var novuService = serviceProvider.GetRequiredService<INovuService>();

        NotificationNewsModel? dataModel = JsonSerializer.Deserialize<NotificationNewsModel>(message);
        if (dataModel == null)
        {
            logger.LogError("Failed to deserialize notification news model");
            return;
        }

        var notificationPayload = new
        {
            title = dataModel.Title,
            content = dataModel.Content,
            linkDetail = dataModel.LinkDetail,
            createdAt = dataModel.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            type = NotificationConstaints.NewNotification
        };
            var novuModel = new NovuTriggerNotificationModel
            {
                WorkflowId = "news-notification",
                Payload = notificationPayload,
                NotificationTriggerType = NotificationTriggerType.SystemWide
            };

            var result = await novuService.TriggerNotification(novuModel);
            if (result)
            {
                logger.LogInformation($"Novu news notification sent to user {dataModel.Title}");
            }
            else
            {
                logger.LogError($"Failed to send Novu news notification");
            }
    }
}