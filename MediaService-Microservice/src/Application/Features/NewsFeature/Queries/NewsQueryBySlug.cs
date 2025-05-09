using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.DaprModel.Document;
using Application.Common.Models.DaprModel.Flashcard;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.NewsModel;
using Application.Constants;
using Application.Services;
using Application.Services.CacheService.Interfaces;
using Dapr.Client;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Domain.QueriesFilter;
using Infrastructure.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;

#pragma warning disable
namespace Application.Features.NewsFeature.Queries;

public record NewsQueryBySlug : IRequest<NewsResponseModel>
{
    public string slug;
}

public class NewsQueryBySlugHandler(MediaDbContext dbContext, IMapper mapper,
    DaprClient daprClient,
    IRedisDistributedCache redisCache, IProducerService producerService) 
    : IRequestHandler<NewsQueryBySlug, NewsResponseModel>
{
    public async Task<NewsResponseModel> Handle(NewsQueryBySlug request, CancellationToken cancellationToken)
    {
        //if (request.slug.IsNullOrEmpty())
        //    return null; // Validate input early to avoid unnecessary DB hits

        //var cacheKey = $"news:id:{request.slug}";
        //var cachedNews = await redisCache.GetStringAsync(cacheKey, token: cancellationToken);
        //if (!string.IsNullOrEmpty(cachedNews))
        //{
        //    var cachedResult = JsonConvert.DeserializeObject<NewsResponseModel>(cachedNews);
        //    if (cachedResult != null)
        //    {
        //        await producerService.ProduceObjectWithKeyAsync(KafkaConstaints.NewViewUpdate, cachedResult.Id.ToString(), cachedResult.Id.ToString());
        //        return cachedResult;
        //    }
        //}
        var now = DateTime.Now;

        // Cache Miss: Fetch from database
        var newsEntity = await dbContext.News
            .Find(n => n.Slug == request.slug && !n.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (newsEntity == null)
            return null;
        var subjectGradeResponse = await daprClient.InvokeMethodAsync<UserResponseMediaDapr>(
               HttpMethod.Get,
               "user-sidecar",
               $"api/v1/dapr/user-media?userId={newsEntity.Author.AuthorId}"
           );
        var author = new Author
        {
            AuthorId = Guid.TryParse(subjectGradeResponse.UserId?.FirstOrDefault(), out var authorId) ? authorId : Guid.Empty,
            Avatar = subjectGradeResponse.Avatar?.FirstOrDefault(),
            FullName = subjectGradeResponse.Username?.FirstOrDefault()
        };
        // Fetch News Tags
        var newsTags = await dbContext.NewsTags
            .Find(t => t.Id == newsEntity.NewsTagId)
            .FirstOrDefaultAsync(cancellationToken);

        // Map to Response Model
        var newsResponse = mapper.Map<NewsResponseModel>(newsEntity);
        newsResponse.NewsTagName = newsTags?.NewTagName;

        // Fetch Additional Data if NewsTag is "Tips" (Parallel Execution)
        if (newsTags?.NewTagName == "Tips")
        {
            var documentResponse = await daprClient.InvokeMethodAsync<DocumentTipsResponseDapr>(
                   HttpMethod.Get,
                   "document-sidecar",
                   $"api/v1/dapr/document-tips?documentIds={string.Join("&documentIds=", newsEntity.DocumentIds.Select(x => x.ToString()))}"
               );
            var flashcardResponse = await daprClient.InvokeMethodAsync<FlashcardTipsResponseDapr>
                (
                    HttpMethod.Get,
                    "document-sidecar",
                    $"api/v1/dapr/flashcard-tips?flashcardIds={string.Join("&flashcardIds=", newsEntity.FlashcardIds.Select(x => x.ToString()))}"
                );

            newsResponse.DocumentList = documentResponse.DocumentId
                .Select((id, index) => new Document
                {
                    DocumentId = Guid.Parse(id),
                    DocumentName = documentResponse.DocumentName[index],
                    DocumentSlug = documentResponse.DocumentSlug[index]
                }).ToList();

            newsResponse.FlashcardList = flashcardResponse.FlaschcardId
                .Select((id, index) => new Flashcard
                {
                    FlashcardId = Guid.Parse(id),
                    FlashcardName = flashcardResponse.FlaschcardName[index],
                    FlashcardSlug = flashcardResponse.FlaschcardSlug[index]
                }).ToList();
            //TheoryTipsRequest theoryRequest = new() { };
            //theoryRequest.TheoryId.AddRange(result.TheoryIds.Select(x => x.ToString()));
            //var theoryResponse = await theoryServiceRpcClient.GetTheoryTipsAsync(theoryRequest);

            //for (int i = 0; i < theoryResponse.TheoryId.Count(); i++)
            //{
            //    Theory theory = new()
            //    {
            //        TheoryId = Guid.Parse(theoryResponse.TheoryId[i]),
            //        TheoryName = theoryResponse.TheoryName[i].ToString(),

            //    };
            //    newsResponse.Theory.Add(theory);
            //}
        }

        // Cache the result with a 24-hour expiration
        //if (newsResponse != null)
        //{
        //    var cacheSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        //    await redisCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(newsResponse, cacheSettings),
        //        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) }, cancellationToken);
        //}

        // Kafka Event After Caching
        await producerService.ProduceObjectWithKeyAsync(KafkaConstaints.NewViewUpdate, newsResponse.Id.ToString(), newsResponse.Id.ToString());

        return newsResponse;
    }
}

