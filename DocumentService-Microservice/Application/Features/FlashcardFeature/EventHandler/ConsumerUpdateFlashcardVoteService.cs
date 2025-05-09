using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.FlashcardFeature.EventHandler;

public class ConsumerUpdateFlashcardVoteService(IConfiguration configuration, ILogger<ConsumerUpdateFlashcardVoteService> logger, IServiceProvider serviceProvider) : KafkaConsumerFlashcardVoteMethod(configuration, logger, serviceProvider, TopicKafkaConstaints.FlashcardVoteUpdate, "flashcard_vote_consumer_group")
{
    private readonly ILogger<ConsumerUpdateFlashcardViewService> _logger;
    private readonly IProducerService _producerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    protected override async Task ProcessMessage(Dictionary<string, (double sum, int count)> message, IServiceProvider serviceProvider)
    {
        if (message is not null)
        {
            foreach (var item in message)
            {
                if (item.Value.count > 0)
                {
                    var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
                    Guid flashcardId = Guid.Parse(item.Key);

                    var flashcardCheck = await unitOfWork.FlashcardRepository.Get(x => x.Id.Equals(flashcardId) && x.DeletedAt == null);
                    var flashcard = flashcardCheck.FirstOrDefault();

                    if (flashcard.Star is null)
                    {
                        flashcard.Star = 0;
                    }
                    if (flashcard.Vote is null)
                    {
                        flashcard.Vote = 0;
                    }
                    flashcard.Star = ((flashcard.Star * flashcard.Vote) + item.Value.sum) / (flashcard.Vote + item.Value.count);
                    
                    flashcard.Vote += item.Value.count;
                    
                    _ = unitOfWork.FlashcardRepository.Update(flashcard);
                    int result = await unitOfWork.SaveChangesAsync();
                    if (result <= 0)
                    {
                        _logger.LogError(ResponseConstaints.FlashcardUpdateFailed + flashcard.Id);
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