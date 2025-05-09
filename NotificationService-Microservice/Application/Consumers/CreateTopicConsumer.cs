using Application.Common.Interfaces;
using Application.Constants;
using SharedProjects.ConsumeModel.NovuModel;
using System.Text.Json;

public class CreateTopicConsumer(
    IConfiguration configuration,
    ILogger<CreateTopicConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBase<NovuTopicModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.NotificationCreateTopic, "notification_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var novuService = serviceProvider.GetRequiredService<INovuService>();

        NovuTopicModel novuTopic = JsonSerializer.Deserialize<NovuTopicModel>(message);

        _ = await novuService.CreateTopic(novuTopic);
    }
}