using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using Algolia.Search.Http;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Common.Models;
using Application.Common.UoW;
using Application.Constants;
using Application.Features.SubjectFeature.Commands;
using Application.KafkaMessageModel;
using AutoMapper;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;
using SharedProject.ConsumeModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Features.DocumentFeature.EventHandler;

public class ConsumerUpdateDocumentViewService(IConfiguration configuration, ILogger<ConsumerUpdateDocumentViewService> logger, IServiceProvider serviceProvider) : KafkaConsumerDocumentViewMethod(configuration, logger, serviceProvider, TopicKafkaConstaints.DocumentViewUpdate, "document_view_consumer_group")
{
    private readonly ILogger<ConsumerUpdateDocumentViewService> _logger;
    private readonly IProducerService _producerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    protected override async Task ProcessMessage(Dictionary<string, int> message, IServiceProvider serviceProvider)
    {
        if (message is not null)
        {
            foreach (var item in message)
            {
                if (item.Value > 0)
                {
                    var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
                    Guid documentId = Guid.Parse(item.Key);
                    var document = await unitOfWork.DocumentRepository.Get(filter: x => x.Id == documentId);
                    var documentUpdateData = document.FirstOrDefault();
                    if (documentUpdateData.View is null)
                    {
                        documentUpdateData.View = 0;
                    }
                    documentUpdateData.View += item.Value;
                    _ = unitOfWork.DocumentRepository.Update(documentUpdateData);
                    int result = await unitOfWork.SaveChangesAsync();
                    if (result <= 0)
                    {
                        _logger.LogError(ResponseConstaints.DocumentUpdateFailed + documentUpdateData.Id);
                    }
                }
            }
        }
    }
}
//_logger.LogError("sdkjghflksjdfhglkjsdhjfgl;jsdl;kfgjl;ksdjfg;l before" + subject.View);
//                    subject.View += data.Count();
//                    var update = unitOfWork.SubjectRepository.Update(subject);
//var result = await unitOfWork.SaveChangesAsync();
//                    if (result > 0)
//                    {
//                        _logger.LogError($"Update Successfully!!!", subject.Id);
//                    }
//                    _logger.LogError($"Update Fail!!!");
//_logger.LogError("sdkjghflksjdfhglkjsdhjfgl;jsdl;kfgjl;ksdjfg;l after" + subject.View);