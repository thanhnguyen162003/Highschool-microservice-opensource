using System.Net;
using System.Text.Json;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Application.Features.SubjectFeature.Commands;
using Application.Features.SubjectFeature.EventHandler;
using Domain.CustomModel;
using Infrastructure.Contexts;
using SharedProject.ConsumeModel;

namespace Application.Consumers.RetryConsumer;

public class SubjectUpdateRetryConsumer(
    IConfiguration configuration,
    ILogger<SubjectUpdateRetryConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBase5Minutes<SubjectUpdateConsumeModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.SubjectImageUpdatedRetry, "subject_update_image_document_consumer_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var sender = serviceProvider.GetRequiredService<ISender>();
        var logger = serviceProvider.GetRequiredService<ILogger<ConsumerUpdateSubjectService>>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();

        SubjectUpdateConsumeModel subjectImageModel = JsonSerializer.Deserialize<SubjectUpdateConsumeModel>(message);
        SubjectModel subjectModel = new SubjectModel()
        {
            Id = subjectImageModel.SubjectId,
            Image = subjectImageModel.ImageUrl,
        };
        var command = new UpdateSubjectCommand()
        {
            SubjectModel = subjectModel
        };
        var result = await sender.Send(command);
        if (result.Status != HttpStatusCode.OK)
        {
			//TODO: Produce to deadletter after retry unsuccess
			var subjectView = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectImageUpdatedRetry, subjectImageModel.SubjectId.ToString(), subjectImageModel);
            logger.LogInformation("RetryQueue in subject update image");
            logger.Log(LogLevel.Error, "Error consume image message from Kafka, maybe timeout");
        }
    }
}
