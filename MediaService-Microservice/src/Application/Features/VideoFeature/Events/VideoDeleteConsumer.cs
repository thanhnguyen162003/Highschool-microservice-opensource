using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Constants;
using Infrastructure.Data;

namespace Application.Features.VideoFeature.Events;

public class VideoDeleteConsumer(IConfiguration configuration, ILogger<VideoDeleteConsumer> logger, IServiceProvider serviceProvider) : KafkaConsumerBase<string>(configuration, logger, serviceProvider, KafkaConstaints.VideoDeleted, "video_delete_consumer_group")
{
    protected override async Task ProcessMessage(string message, IServiceProvider serviceProvider)
    {
        /*var _context = serviceProvider.GetRequiredService<MediaDbContext>();
        var _producerService = serviceProvider.GetRequiredService<IProducerService>();

        var videoId = message;

        var video = await _context.Videos.Find(x => x.Id == videoId).FirstOrDefaultAsync();
        if (video == null)
        {
            return;
        }

        await _context.Videos.DeleteOneAsync(x => x.Id == videoId);
        var model = new KafkaVideoDeletedModel()
        {
            VideoId = videoId
        };
        await _producerService.ProduceObjectAsync(KafkaConstaints.VideoDeleted, model);*/
    }
}
