using System.Net;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using SharedProjects.ConsumeModel.NovuModel;

namespace Application.Consumers.NotificationGroup;

public class UpdateSubscriberConsumer(
    IConfiguration configuration,
    ILogger<UpdateSubscriberConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBase<NovuSubcriberModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.NotificationUpdateSubscriber, "notification_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var novuService = serviceProvider.GetRequiredService<INovuService>();
        var logger = serviceProvider.GetRequiredService<ILogger<NovuSubcriberModel>>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();

        NovuSubcriberModel novuSubscriber = JsonSerializer.Deserialize<NovuSubcriberModel>(message);

        _ = await novuService.UpdateSubcriber(novuSubscriber);
    }
}