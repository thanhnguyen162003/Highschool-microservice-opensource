using System.Text.Json;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Contexts;
using SharedProject.ConsumeModel;

namespace Application.Consumers.RetryConsumer;

public class LessonVideoRetryConsumer(
    IConfiguration configuration,
    ILogger<LessonVideoRetryConsumer> logger,
    IServiceProvider serviceProvider)
    : KafkaConsumerBase5Minutes<KafkaVideoLessonUploadedModel>(configuration, logger, serviceProvider,
        TopicKafkaConstaints.VideoUploadedRetry, "lesson_video_uploaded_consumer_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<DocumentDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<LessonVideoRetryConsumer>>();
        var producer = serviceProvider.GetRequiredService<IProducerService>();


        KafkaVideoLessonUploadedModel lessonModel = JsonSerializer.Deserialize<KafkaVideoLessonUploadedModel>(message);
        var lesson = await context.Lessons.Where(l => l.Id == lessonModel.LessonId && l.DeletedAt == null).FirstOrDefaultAsync();
        lesson.VideoUrl = lessonModel.VideoUrl;
        context.Lessons.Update(lesson);
        var result = await context.SaveChangesAsync();
        if (result <= 0)
        {
            //TODO: Produce to deadletter after retry unsuccess
            await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.VideoUploadedRetry, lessonModel.LessonId.ToString(),lessonModel);
            logger.LogInformation("RetryQueue many time but fail in lesson update video");
            logger.Log(LogLevel.Error, "Error consume image message from Kafka");
        }
    }
}
