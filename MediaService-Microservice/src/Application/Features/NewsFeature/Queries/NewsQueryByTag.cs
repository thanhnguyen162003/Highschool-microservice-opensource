using System.Text.RegularExpressions;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.NewsModel;
using Application.Services.CacheService.Interfaces;
using Dapr.Client;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Domain.QueriesFilter;
using Infrastructure.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

#pragma warning disable
namespace Application.Features.NewsFeature.Queries;

public record NewsQueryByTag : IRequest<PagedList<NewsResponseModel>>
{
    public NewsTagQueryFilter QueryFilter;
    public Guid NewsTagId;
}

public class NewsQueryByTagHandler(MediaDbContext dbContext, IMapper mapper,
    IOptions<PaginationOptions> paginationOptions,
    DaprClient daprClient,
    IRedisDistributedCache redisCache) 
    : IRequestHandler<NewsQueryByTag, PagedList<NewsResponseModel>>
{ 
    public async Task<PagedList<NewsResponseModel>> Handle(NewsQueryByTag request, CancellationToken cancellationToken)
    {
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? paginationOptions.Value.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? paginationOptions.Value.DefaultPageSize : request.QueryFilter.PageSize;
        request.QueryFilter.Search = request.QueryFilter.Search ?? string.Empty;
        var normalInput = Regex.Replace(request.QueryFilter.Search.Trim(), @"\s+", " ").ToLower();

        // Check Redis cache
        // Generate cache key based on query parameters
        //var cacheKey = $"news:{request.QueryFilter.PageNumber}:{request.QueryFilter.PageSize}:{request.QueryFilter.Search}";
        //var cachedNews = await redisCache.GetStringAsync(cacheKey, token: cancellationToken);
        //if (!string.IsNullOrEmpty(cachedNews))
        //{
        //    var cachedResult = JsonConvert.DeserializeObject<PagedList<NewsResponseModel>>(cachedNews);
        //    if (cachedResult != null)
        //    {
        //        return PagedList<NewsResponseModel>.Create(cachedResult, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        //    }
        //}

        //cache miss
        var filter = Builders<News>.Filter.Eq(n => n.IsDeleted, false) &
                 Builders<News>.Filter.Eq(n => n.NewsTagId, request.NewsTagId) &
                 Builders<News>.Filter.Regex(n => n.NewName, new BsonRegularExpression(normalInput, "i"));

        // Apply Sorting & Pagination in MongoDB
        var sortDefinition = Builders<News>.Sort.Descending(n => n.CreatedAt);
        var newsList = await dbContext.News
            .Find(filter)
            .Sort(sortDefinition)
            .Skip((request.QueryFilter.PageNumber - 1) * request.QueryFilter.PageSize)
            .Limit(request.QueryFilter.PageSize)
            .ToListAsync(cancellationToken);

        // Get total count
        var totalCount = await dbContext.News.CountDocumentsAsync(filter);

        if (!newsList.Any())
        {
            return new PagedList<NewsResponseModel>([], 0, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        }
        var subjectGradeResponse = await daprClient.InvokeMethodAsync<UserResponseMediaDapr>(
                      HttpMethod.Get,
                      "user-sidecar",
                      $"api/v1/dapr/user-media"
                  );
        var authorLookup = subjectGradeResponse.UserId
             .Select((id, index) => new Author
             {
                 AuthorId = Guid.Parse(id),
                 FullName = subjectGradeResponse.Username[index],
                 Avatar = subjectGradeResponse.Avatar[index]
             })
             .ToDictionary(a => a.AuthorId, a => a);
        // Fetch News Tags
        var newsTagIds = newsList.Select(n => n.NewsTagId).Distinct().ToList();
        var newsTags = (await dbContext.NewsTags
                .Find(Builders<NewsTag>.Filter.In(t => t.Id, newsTagIds))
                .ToListAsync(cancellationToken))
            .ToDictionary(t => t.Id, t => t.NewTagName);

        // Map News to Response Model
        var responseList = newsList.Select(n => new NewsResponseModel
        {
            Id = n.Id,
            Author = authorLookup.GetValueOrDefault(n.Author.AuthorId),
            NewsTagId = n.NewsTagId,
            Content = n.Content,
            ContentHtml = n.ContentHtml,
            CreatedAt = n.CreatedAt,
            CreatedBy = n.CreatedBy,
            Image = n.Image,
            IsDeleted = n.IsDeleted,
            Location = n.Location,
            NewName = n.NewName,
            Slug = n.Slug,
            UpdatedAt = n.UpdatedAt.GetValueOrDefault(),
            UpdatedBy = n.UpdatedBy.GetValueOrDefault(),
            View = n.TotalView,
            NewsTagName = newsTags.GetValueOrDefault(n.NewsTagId, "Unknown"),
        }).ToList();

        return new PagedList<NewsResponseModel>(responseList, (int)totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);

        // Cache the result with a 24-hour expiration
        //var loopHandling = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        //await redisCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(pagedResult, loopHandling),
        //    new DistributedCacheEntryOptions
        //    {
        //        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        //    }, token: cancellationToken);

    }

  
}
