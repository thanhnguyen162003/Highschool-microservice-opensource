using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.EventHandler;

public class ConsumerUpdateFlashcardViewService(IConfiguration configuration, ILogger<ConsumerUpdateFlashcardViewService> logger, IServiceProvider serviceProvider) : KafkaConsumerFlashcardViewMethod(configuration, logger, serviceProvider, TopicKafkaConstaints.FlashcardViewUpdate, "flashcard_view_consumer_group")
{
    private readonly ILogger<ConsumerUpdateFlashcardViewService> _logger;
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
                    Guid flashcardId = Guid.Parse(item.Key);
                    var flashcardCheck = await unitOfWork.FlashcardRepository.Get(x => x.Id.Equals(flashcardId) && x.DeletedAt == null);
                    var flashcard = flashcardCheck.FirstOrDefault();
                    if (flashcard.TodayView is null)
                    {
                        flashcard.TodayView = 0;
                    }
                    flashcard.TodayView += item.Value;
                    if (flashcard.TotalView is null)
                    {
                        flashcard.TotalView = 0;
                    }
                    flashcard.TotalView += item.Value;
                    _ = unitOfWork.FlashcardRepository.Update(flashcard);
                    int result = await unitOfWork.SaveChangesAsync();
                    if (result <= 0)
                    {
                        _logger.LogError(ResponseConstaints.FlashcardUpdateFailed + flashcard.Id);
                    }
                }
            }
        }
        var nowUtc7 = DateTime.UtcNow.AddHours(7);
        if (nowUtc7.Hour == 23 && nowUtc7.Minute >= 54)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            var list = await unitOfWork.FlashcardRepository.Get(filter: x=>x.TodayView > 0);
            foreach (var item in list) 
            {
                item.TodayView = 0;
                _ = unitOfWork.FlashcardRepository.Update(item);
                int result = await unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    _logger.LogError(ResponseConstaints.FlashcardUpdateFailed + item.Id);
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