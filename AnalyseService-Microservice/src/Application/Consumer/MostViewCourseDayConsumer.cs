using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Data;
using SharedProject.Constaints;

namespace Application.Consumer;

public class MostViewCourseDayConsumer(
    IConfiguration configuration,
    ILogger<MostViewCourseDayConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBase<string>(configuration, logger, serviceProvider, TopicKafkaConstaints.SubjectViewUpdate,
        ConsumerGroup.DayAnalyzeGroup)
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AnalyseDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<MostViewCourseDayConsumer>>();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
    }
}
