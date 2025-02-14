using System.Net;
using System.Text.Json;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Application.Features.DocumentFeature.Commands;
using Infrastructure.Contexts;
using SharedProject.ConsumeModel;

namespace Application.Consumers.RetryConsumer;

public class DocumentUpdateRetryConsumer(
    IConfiguration configuration,
    ILogger<DocumentUpdateRetryConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBase5Minutes<DocumentUpdateConsumeModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.DocumentFileUpdatedRetry, "document_file_consumer_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var sender = serviceProvider.GetRequiredService<ISender>();
        var logger = serviceProvider.GetRequiredService<ILogger<DocumentUpdateConsumeModel>>();
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
        var result = await sender.Send(command);
        if (result.Status != HttpStatusCode.OK)
        {
			//TODO: Produce to deadletter after retry unsuccess
			var subjectView = await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.DocumentFileUpdatedRetry, documentFileUpdate.DocumentId.ToString(), documentFileUpdate);
            logger.LogInformation("RetryQueue in document update file");
            logger.Log(LogLevel.Error, "Error consume file message from Kafka, maybe timeout");
        }
    }
}
