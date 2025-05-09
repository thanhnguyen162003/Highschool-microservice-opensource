using Application.Common.Kafka;
using Application.Features.DocumentFeature.Commands;
using SharedProject.ConsumeModel;
using System.Net;
using System.Text.Json;
using Application.Constants;
using static Confluent.Kafka.ConfigPropertyNames;
using Application.Common.Interfaces.KafkaInterface;

namespace Application.Features.DocumentFeature.EventHandlers;

public class ConsumerUpdateDocumentService(IConfiguration configuration, ILogger<ConsumerUpdateDocumentService> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<DocumentUpdateConsumeModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.DocumentFileUpdated, "document_file_consumer_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var _sender = serviceProvider.GetRequiredService<ISender>();
        var _logger = serviceProvider.GetRequiredService<ILogger<DocumentUpdateConsumeModel>>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();


        DocumentUpdateConsumeModel documentFileUpdate = JsonSerializer.Deserialize<DocumentUpdateConsumeModel>(message);
        var command = new UpdateDocumentCommand()
        {
            DocumentId = documentFileUpdate.DocumentId,
            UpdateDocumentRequestModel = new Common.Models.DocumentModel.UpdateDocumentRequestModel()
            {
                DocumentFileName = documentFileUpdate.DocumentFileName,
            }
        };
        var result = await _sender.Send(command);
        if (result.Status != HttpStatusCode.OK)
        {
            var subjectView = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DocumentFileUpdatedRetry, documentFileUpdate.DocumentId.ToString(), documentFileUpdate);
            _logger.LogInformation("RetryQueue in document update file");
            _logger.Log(LogLevel.Error, "Error consume file message from Kafka, maybe timeout");
        }
    }
}