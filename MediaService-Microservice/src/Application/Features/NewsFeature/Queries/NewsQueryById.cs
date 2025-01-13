using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.NewsModel;
using Application.Constants;
using Application.Services;
using Application.Services.CacheService.Interfaces;
using Confluent.Kafka;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Domain.QueriesFilter;
using Infrastructure.Data;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using Newtonsoft.Json;

#pragma warning disable
namespace Application.Features.NewsFeature.Queries;

public record NewsQueryById : IRequest<NewsResponseModel>
{
    public Guid newsId;
}

public class NewsQueryByIdHandler(MediaDbContext dbContext,
    IMapper mapper,
    UserServiceRpc.UserServiceRpcClient client,
    IRedisDistributedCache redisCache, IProducerService producerService) 
    : IRequestHandler<NewsQueryById, NewsResponseModel>
{    
    public async Task<NewsResponseModel> Handle(NewsQueryById request, CancellationToken cancellationToken)
    {
        var cacheKey = $"news:id:{request.newsId}";
        var cachedNews = await redisCache.GetStringAsync(cacheKey, token: cancellationToken);
        if (!string.IsNullOrEmpty(cachedNews))
        {
            var cachedResult = JsonConvert.DeserializeObject<NewsResponseModel>(cachedNews);
            if (cachedResult != null)
            {
                await producerService.ProduceObjectWithKeyAsync(KafkaConstaints.NewViewUpdate, cachedResult.Id.ToString(), cachedResult.Id.ToString());
                return cachedResult;
            }
        }

        
        //cache miss
        var result = await dbContext.News.Find(n => n.Id.Equals(request.newsId) && n.IsDeleted == false).ToListAsync(cancellationToken: cancellationToken);
        if (!result.Any())
        {
            return null;
        }

        //grpc
        UserMediaRequest userRequest = new() { UserId = result.FirstOrDefault().AuthorId.ToString() };
        var subjectGradeResponse = await client.GetUserMediaAsync(userRequest);
        Author author = new Author();
        if (subjectGradeResponse.UserId.Count != 0
            && subjectGradeResponse.Image.Count != 0
            && subjectGradeResponse.Username.Count != 0)
        {
            author.AuthorId = Guid.Parse(subjectGradeResponse.UserId[0].ToString());
            author.AuthorImage = subjectGradeResponse.Image[0].ToString();
            author.AuthorName = subjectGradeResponse.Username[0].ToString();
        }

        var newsTagIds = result.Select(n => n.NewsTagId).Distinct().ToList();
        var newsTags = await dbContext.NewsTags.Find(Builders<NewsTag>.Filter.In(t => t.Id, newsTagIds)).ToListAsync(cancellationToken);

        var newsResponse = result.Select(n => new NewsResponseModel
        {
            Id = n.Id,
            Author = author,
            NewsTagId = n.NewsTagId,
            Content = n.Content,
            ContentHtml = n.ContentHtml,
            CreatedAt = n.CreatedAt,
            CreatedBy = n.CreatedBy,
            Image = n.Image,
            IsDeleted = n.IsDeleted,
            NewName = n.NewName,
            Location = n.Location,
            Slug = n.Slug,
            UpdatedAt = n.UpdatedAt.Value,
            UpdatedBy = n.UpdatedBy.Value,
            View = n.TotalView,
            NewsTagName = newsTags.FirstOrDefault(t => t.Id == n.NewsTagId)?.NewTagName
        }).FirstOrDefault();

        // Cache the result with a 24-hour expiration
        if (newsResponse != null)
        {
            var loopHandling = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            await redisCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(newsResponse, loopHandling), 
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                }, token: cancellationToken);
        }
        await producerService.ProduceObjectWithKeyAsync(KafkaConstaints.NewViewUpdate, newsResponse.Id.ToString(), newsResponse.Id.ToString());
        return newsResponse;
    }
}

