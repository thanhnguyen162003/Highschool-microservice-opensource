using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubjectFeature.EventHandler;

public class ConsumerUpdateSubjectViewService(IConfiguration configuration, ILogger<ConsumerUpdateSubjectViewService> logger, IServiceProvider serviceProvider) : KafkaConsumerSubjectViewMethod(configuration, logger, serviceProvider, TopicKafkaConstaints.SubjectViewUpdate, "subject_view_consumer_group")
{
    private readonly ILogger<ConsumerUpdateSubjectViewService> _logger;
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
                    Guid subjectId = Guid.Parse(item.Key);
                    var subject = await unitOfWork.SubjectRepository.Get(filter: x => x.Id == subjectId);
                    var subjectUpdateData = subject.FirstOrDefault();
                    if (subjectUpdateData.View is null)
                    {
                        subjectUpdateData.View = 0;
                    }
                    subjectUpdateData.View += item.Value;
                    var result = await unitOfWork.SubjectRepository.UpdateSubject(subjectUpdateData);
                    if (result is false)
                    {
                        _logger.LogError("Update Fail" + subjectUpdateData.Id);
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