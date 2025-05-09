using System.Text.Json;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Application.Features.SubjectFeature.Events;
using Application.KafkaMessageModel;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Bson;
using SharedProject.Models;

namespace Application.Consumers.RetryConsumer;

public class SubjectCreateRetryConsumer(IConfiguration configuration, ILogger<SubjectCreateRetryConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase5Minutes<SubjectImageCreateModel>(configuration, logger, serviceProvider, KafkaConstaints.SubjectImageCreatedRetry, "subject_update_consumer_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<MediaDbContext>();
        var producerService = serviceProvider.GetRequiredService<IProducerService>();
        // var _grpcServiceSubject = serviceProvider.GetRequiredService<SubjectServiceCheckRpc.SubjectServiceCheckRpcClient>();

        SubjectImageCreateModel subject = JsonSerializer.Deserialize<SubjectImageCreateModel>(message);
        //check subjectId exits???
        // SubjectExitsRequest subjectCheckRequest = new SubjectExitsRequest
        // {
        //     SubjectId = { subject.SubjectId.ToString() }
        // };
        //
        // var subjectExit = await _grpcServiceSubject.CheckSubjectExitAsync(subjectCheckRequest);
        //
        // if (subjectExit.IsSubjectExit.ToString().Equals("false"))
        // {
        //     return;
        // }

        SubjectImage subjectModel = new SubjectImage()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            CreatedAt = DateTime.Now,
            SubjectId = subject.SubjectId,
            SubjectImageUrl = subject.ImageUrl,
            UpdatedAt = DateTime.Now,
            Format = subject.Format,
            PublicIdUrl = subject.PublicIdUrl
        };
        KafkaSubjectUpdatedImageModel model = new KafkaSubjectUpdatedImageModel()
        {
            SubjectId = subject.SubjectId,
            ImageUrl = subject.ImageUrl
        };
        await context.SubjectImages.InsertOneAsync(subjectModel);
        var producer = await producerService.ProduceObjectWithKeyAsync(KafkaConstaints.SubjectImageUpdated, subject.SubjectId.ToString(), model);
        if (!producer)
        {
            var _logger = serviceProvider.GetRequiredService<ILogger<SubjectCreateConsumer>>();
            _ = await producerService.ProduceObjectWithKeyAsync(KafkaConstaints.SubjectImageCreatedRetry, subject.SubjectId.ToString(), model);
            _logger.LogInformation("RetryQueue in SubjectImage");
            _logger.Log(LogLevel.Error, "Error consume SubjectImage from Kafka, maybe timeout");
        }
    }
}
