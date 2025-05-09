using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Interfaces;
using Application.Common.Kafka;
using Application.Constants;
using SharedProjects.ConsumeModel.NovuModel;
using System.Text.Json;

public class TriggerNotificationConsumer(
    IConfiguration configuration,
    ILogger<TriggerNotificationConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBase<NovuTriggerNotificationModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.NotificationTriggerNotification, "notification_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var novuService = serviceProvider.GetRequiredService<INovuService>();
        var logger = serviceProvider.GetRequiredService<ILogger<NovuTriggerNotificationModel>>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();

        NovuTriggerNotificationModel novuTrigger = JsonSerializer.Deserialize<NovuTriggerNotificationModel>(message);

        _ = await novuService.TriggerNotification(novuTrigger);
    }
}