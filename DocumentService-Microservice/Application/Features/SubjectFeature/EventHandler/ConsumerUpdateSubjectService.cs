using System.Net;
using System.Text.Json;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Application.Features.SubjectFeature.Commands;
using Domain.CustomModel;
using SharedProject.ConsumeModel;

namespace Application.Features.SubjectFeature.EventHandler;

public class ConsumerUpdateSubjectService(IConfiguration configuration, ILogger<ConsumerUpdateSubjectService> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<SubjectUpdateConsumeModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.SubjectImageUpdated, "subject_update_image_document_consumer_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var _sender = serviceProvider.GetRequiredService<ISender>();
        var _logger = serviceProvider.GetRequiredService<ILogger<ConsumerUpdateSubjectService>>();
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
        var result = await _sender.Send(command);
        if (result.Status != HttpStatusCode.OK)
        {
            var subjectView = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.SubjectImageUpdated, subjectImageModel.SubjectId.ToString(), subjectImageModel);
            _logger.LogInformation("RetryQueue in subject update image");
            _logger.Log(LogLevel.Error, "Error consume image message from Kafka, maybe timeout");
        }
    }
}
