using System.Net;
using System.Threading;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Kafka;
using Application.Common.Models;
using Application.Constants;
using Domain.Entities.SqlEntites;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.NewsFeature.EventHandler;

public class ConsumerUpdateNewViewService(IConfiguration configuration, ILogger<ConsumerUpdateNewViewService> logger, IServiceProvider serviceProvider) : KafkaConsumerNewsViewMethod(configuration, logger, serviceProvider, KafkaConstaints.NewViewUpdate, "new_view_consumer_group")
{
    private readonly ILogger<ConsumerUpdateNewViewService> _logger;
    private readonly IProducerService _producerService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    protected override async Task ProcessMessage(Dictionary<string, int> message, IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<MediaDbContext>();
        var claimInterface = serviceProvider.GetRequiredService<IClaimInterface>();

        if (message is not null)
        {

            foreach (var item in message)
            {
                if (item.Value > 0)
                {

                    Guid subjectId = Guid.Parse(item.Key);
                    var news = dbContext.News.Find(x => x.Id == subjectId);
                    var newsUpdateData = news.FirstOrDefault();
                    if (newsUpdateData.TodayView is null)
                    {
                        newsUpdateData.TodayView = 0;
                    }
                    newsUpdateData.TodayView += item.Value;
                    newsUpdateData.TotalView += item.Value;

                    var filter = Builders<News>.Filter.Eq(n => n.Id, newsUpdateData.Id);
                    var update = Builders<News>.Update
                        .Set(n => n.NewsTagId, newsUpdateData.NewsTagId)
                        .Set(n => n.NewName, newsUpdateData.NewName)
                        .Set(n => n.ContentHtml, newsUpdateData.ContentHtml)
                        .Set(n => n.Content, newsUpdateData.Content)
                        .Set(n => n.Slug, newsUpdateData.Slug)
                        .Set(n => n.Image, newsUpdateData.Image)
                        .Set(n => n.TotalView, newsUpdateData.TotalView)
                        .Set(n => n.TodayView, newsUpdateData.TodayView)
                        .Set(n => n.Location, newsUpdateData.Location)
                        .Set(n => n.UpdatedAt, DateTime.UtcNow)
                        .Set(n => n.UpdatedBy, newsUpdateData.UpdatedBy);

                    var result = await dbContext.News.UpdateOneAsync(filter, update);
                    if (result.ModifiedCount <= 0)
                    {
                        _logger.LogError("Update Fail" + newsUpdateData.Id);
                    }
                }

            }
        }
            
        var nowUtc7 = DateTime.UtcNow.AddHours(7);
        if (nowUtc7.Hour == 23 && nowUtc7.Minute >= 30)
        {
            var list = dbContext.News.Find(x => x.TotalView > 0).ToList();
            foreach (var newsUpdateData in list)
            {
                newsUpdateData.TodayView = 0;
                var filter = Builders<News>.Filter.Eq(n => n.Id, newsUpdateData.Id);
                var update = Builders<News>.Update
                    .Set(n => n.NewsTagId, newsUpdateData.NewsTagId)
                    .Set(n => n.NewName, newsUpdateData.NewName)
                    .Set(n => n.ContentHtml, newsUpdateData.ContentHtml)
                    .Set(n => n.Content, newsUpdateData.Content)
                    .Set(n => n.Slug, newsUpdateData.Slug)
                    .Set(n => n.Image, newsUpdateData.Image)
                    .Set(n => n.TotalView, newsUpdateData.TotalView)
                    .Set(n => n.TodayView, newsUpdateData.TodayView)
                    .Set(n => n.Location, newsUpdateData.Location)
                    .Set(n => n.UpdatedAt, DateTime.UtcNow)
                    .Set(n => n.UpdatedBy, newsUpdateData.UpdatedBy)
                    .Set(n => n.Hot, newsUpdateData.Hot);
                var result = await dbContext.News.UpdateOneAsync(filter, update);
                if (result.ModifiedCount <= 0)
                {
                    _logger.LogError("Update Fail" + newsUpdateData.Id);
                }
            }
        }
    }
}
