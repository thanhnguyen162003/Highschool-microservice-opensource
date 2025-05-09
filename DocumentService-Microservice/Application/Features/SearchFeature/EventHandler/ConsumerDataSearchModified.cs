using Application.Common.Kafka;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.SearchModel;
using Application.Common.Models.SubjectModel;
using Application.Constants;
using Application.Services.SearchService;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Application.Features.SearchFeature.EventHandler
{
    public class ConsumerDataSearchModified(IConfiguration configuration, ILogger<ConsumerDataSearchModified> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<ConsumerDataSearchModified>(configuration, logger, serviceProvider, TopicKafkaConstaints.DataSearchModified, "document_service_group_test_1")
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<ConsumerDataSearchModified> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var logger = scopedProvider.GetRequiredService<ILogger<ConsumerDataSearchModified>>();
            var unitOfWork = scopedProvider.GetRequiredService<IUnitOfWork>();
            var mapper = scopedProvider.GetRequiredService<IMapper>();
            var algoliaService = scopedProvider.GetRequiredService<IAlgoliaService>();

            try
            {
                var result = false;
                var data = JsonConvert.DeserializeObject<SearchEventDataModifiedModel>(message);
                if (data == null)
                {
                    logger.LogError("Failed to deserialize message: {Message}", message);
                    return;
                }

                if(data.IndexName != IndexName.course)
                {
                    foreach (var item in data.Data!)
                    {
                        dynamic? entity = data.IndexName switch
                        {
                            IndexName.folder => mapper.Map<FolderUserResponse>(unitOfWork.FolderUserRepository.GetByID(item.Id)),
                            IndexName.document => mapper.Map<DocumentResponseModel>(unitOfWork.DocumentRepository.GetByID(item.Id)),
                            IndexName.flashcard => mapper.Map<FlashcardResponseModel>(unitOfWork.FlashcardRepository.GetByID(item.Id)),
                            IndexName.subject => mapper.Map<SubjectResponseModel>(unitOfWork.SubjectRepository.GetByID(item.Id)),
                            _ => null
                        };

                        if (entity == null)
                        {
                            logger.LogWarning("Entity not found for Id {Id} in index {IndexName}", item.Id, data.IndexName);
                            return;
                        }

                        result = await HandleAlgoliaOperations(data, entity, algoliaService);

                        if (result)
                            logger.LogInformation("Successfully processed message: {Message}", message);
                        else
                            logger.LogError("Failed to process message: {Message}", message);
                    }
                } else
                {
                    foreach (var item in data.Data!)
                    {
                        switch (data.Type)
                        {
                            case TypeEvent.Create:
                            case TypeEvent.Update:
                                result = await algoliaService.AddOrUpdateCourseRecord(item.TypeField!.Value, item.Id, item.Name!);
                                break;
                            case TypeEvent.Delete:
                                result = await algoliaService.DeleteCourseRecord(item.TypeField!.Value, item.Id);
                                break;
                        }

                        if (result)
                            logger.LogInformation("Successfully processed message: {Message}", message);
                        else
                            logger.LogError("Failed to process message: {Message}", message);
                    }
                }
            } catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message");
            }
        }

        private async Task<bool> HandleAlgoliaOperations(SearchEventDataModifiedModel data, dynamic entity, IAlgoliaService algoliaService)
        {
            bool result = false;

            switch (data.Type)
            {
                case TypeEvent.Create:
                case TypeEvent.Update:
                    result = await algoliaService.AddOrUpdateRecord(data.IndexName, entity.ObjectID, entity);
                    if (result)
                        result = await algoliaService.AddOrUpdateRecord(IndexName.name, entity.ObjectID, new { objectID = entity.ObjectID, name = entity.Name });
                    break;

                case TypeEvent.Delete:
                    result = await algoliaService.DeleteRecord(data.IndexName, entity.ObjectID);
                    if (result)
                        result = await algoliaService.DeleteRecord(IndexName.name, entity.ObjectID);
                    break;
            }

            return result;
        }

    }
}
