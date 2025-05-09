using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Interfaces;
using Application.Common.Kafka;
using Application.Constants;
using SharedProjects.ConsumeModel.NovuModel;
using System.Text.Json;

public class AddUserToTopicConsumer(
    IConfiguration configuration,
    ILogger<AddUserToTopicConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBase<NovuSubcriberTopicModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.NotificationAddUserToTopic, "notification_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var novuService = serviceProvider.GetRequiredService<INovuService>();
        var logger = serviceProvider.GetRequiredService<ILogger<NovuSubcriberTopicModel>>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();

        NovuSubcriberTopicModel novuSubscriberTopic = JsonSerializer.Deserialize<NovuSubcriberTopicModel>(message);

        _ = await novuService.AddUserToTopic(novuSubscriberTopic);
    }
}