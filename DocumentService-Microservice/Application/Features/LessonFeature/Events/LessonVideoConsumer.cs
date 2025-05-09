using System.Text.Json;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Contexts;
using SharedProject.ConsumeModel;

namespace Application.Features.LessonFeature.Events;

public class LessonVideoConsumer(IConfiguration configuration, ILogger<LessonVideoConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<KafkaVideoLessonUploadedModel>(configuration, logger, serviceProvider, TopicKafkaConstaints.VideoUploaded, "lesson_video_uploaded_consumer_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
{
    int retryCount = 0;
    const int maxRetries = 2; 
    bool success = false;
    var context = serviceProvider.GetRequiredService<DocumentDbContext>();
    var logger = serviceProvider.GetRequiredService<ILogger<LessonVideoConsumer>>();
    var producer = serviceProvider.GetRequiredService<IProducerService>();

    KafkaVideoLessonUploadedModel lessonModel = JsonSerializer.Deserialize<KafkaVideoLessonUploadedModel>(message);

    while (retryCount <= maxRetries && !success)
    {
        try
        {
            var lesson = await context.Lessons
                .Where(l => l.Id == lessonModel.LessonId && l.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (lesson is not null)
            {
                lesson.VideoUrl = lessonModel.VideoUrl;
                context.Lessons.Update(lesson);
                var result = await context.SaveChangesAsync();
                
                if (result > 0)
                {
                    success = true;
                    logger.LogInformation("Successfully updated lesson with video URL.");
                }
                else
                {
                    throw new Exception("Failed to save changes to the database.");
                }
            }
            else
            {
                throw new Exception("Lesson not found or has been deleted.");
            }
        }
        catch (Exception ex)
        {
            retryCount++;
            logger.LogError(ex, $"An error occurred while processing the message. Attempt {retryCount} of {maxRetries + 1}.");

            if (retryCount > maxRetries)
            {
                await producer.ProduceObjectWithKeyAsync(TopicKafkaConstaints.VideoUploadedRetry, lessonModel.LessonId.ToString(), lessonModel);
                logger.LogInformation("Message sent to retry queue due to repeated failures.");
            }
        }
    }
}
}
