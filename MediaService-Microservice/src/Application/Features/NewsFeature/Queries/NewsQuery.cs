using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Amazon.S3.Model;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.NewsModel;
using Application.Services.CacheService.Interfaces;
using Confluent.Kafka;
using Dapr.Client;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Domain.QueriesFilter;
using Infrastructure.Data;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

#pragma warning disable
namespace Application.Features.NewsFeature.Queries;

public record NewsQuery : IRequest<PagedList<NewsResponseModel>>
{
    public NewsQueryFilter QueryFilter;
}

public class NewsQueryHandler(MediaDbContext dbContext,
    IMapper mapper,
    DaprClient daprClient,
    IOptions<PaginationOptions> paginationOptions, 
    IRedisDistributedCache redisCache) : IRequestHandler<NewsQuery, PagedList<NewsResponseModel>>
{

    public async Task<PagedList<NewsResponseModel>> Handle(NewsQuery request, CancellationToken cancellationToken)
    {
        bool fetchAll = request.QueryFilter.PageNumber == -1;
        // Set default values
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? paginationOptions.Value.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? paginationOptions.Value.DefaultPageSize : request.QueryFilter.PageSize;
        request.QueryFilter.Search = request.QueryFilter.Search ?? string.Empty;
        var normalInput = Regex.Replace(request.QueryFilter.Search.Trim(), @"\s+", " ").ToLower();


        //// Generate cache key based on query parameters
        //var cacheKey = $"news:{request.QueryFilter.PageNumber}:{request.QueryFilter.PageSize}:{request.QueryFilter.Search}";

        //// Try to get data from Redis cache
        //var cachedNews = await redisCache.GetStringAsync(cacheKey, token: cancellationToken);
        //if (!string.IsNullOrEmpty(cachedNews))
        //{
        //    //Deserialize cached result
        //   var cachedResult = JsonConvert.DeserializeObject<PagedList<NewsResponseModel>>(cachedNews);
        //    if (cachedResult != null)
        //    {
        //        return PagedList<NewsResponseModel>.Create(SortResponse(cachedResult.AsQueryable(), request.QueryFilter), request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        //    }
        //}


        var filter = Builders<News>.Filter.Eq(n => n.IsDeleted, false) &
                     Builders<News>.Filter.Regex(n => n.NewName, new BsonRegularExpression(normalInput, "i"));

        if (!string.IsNullOrEmpty(request.QueryFilter.NewsTagId?.ToString()))
        {
            filter &= Builders<News>.Filter.Eq(n => n.NewsTagId, request.QueryFilter.NewsTagId);
        }

        if (!string.IsNullOrEmpty(request.QueryFilter.Location))
        {
            filter &= Builders<News>.Filter.Regex(n => n.Location, new BsonRegularExpression(request.QueryFilter.Location, "i"));
        }// Sorting (handled in MongoDB)
        var sortDefinition = request.QueryFilter.Sort?.ToLower() switch
        {
            "date" => request.QueryFilter.Direction?.ToLower() == "desc"
                ? Builders<News>.Sort.Descending(n => n.CreatedAt)
                : Builders<News>.Sort.Ascending(n => n.CreatedAt),
            "view" => request.QueryFilter.Direction?.ToLower() == "desc"
                ? Builders<News>.Sort.Descending(n => n.TotalView)
                : Builders<News>.Sort.Ascending(n => n.TotalView),
            _ => Builders<News>.Sort.Descending(n => n.CreatedAt) // Default sorting
        };
        var query = dbContext.News
            .Find(filter)
            .Sort(sortDefinition);
        if (!fetchAll)
        {
            query = query.Skip((request.QueryFilter.PageNumber - 1) * request.QueryFilter.PageSize)
            .Limit(request.QueryFilter.PageSize);
        }
        var newsList = await query.ToListAsync(cancellationToken);
        // Fetch filtered and sorted data from MongoDB


        // Fetch total count for pagination
        var totalCount = fetchAll ? newsList.Count : await dbContext.News.CountDocumentsAsync(filter, cancellationToken : cancellationToken);

        if (!newsList.Any())
        {
            return new PagedList<NewsResponseModel>([], 0, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        }

        // Fetch News Tags
        var newsTagIds = newsList.Select(n => n.NewsTagId).Distinct().ToList();
        var newsTags = (await dbContext.NewsTags
                .Find(Builders<NewsTag>.Filter.In(t => t.Id, newsTagIds))
                .ToListAsync(cancellationToken))
            .ToDictionary(t => t.Id, t => t.NewTagName);


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


        // Map News to Response Model
        var mapperList = newsList.Select(n => new NewsResponseModel
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
            NewName = n.NewName,
            Location = n.Location,
            Slug = n.Slug,
            UpdatedAt = n.UpdatedAt.GetValueOrDefault(),
            UpdatedBy = n.UpdatedBy.GetValueOrDefault(),
            View = n.TotalView,
            NewsTagName = newsTags.GetValueOrDefault(n.NewsTagId, "Unknown"),
        }).ToList();
        // Cache the result with a TTL
        //await redisCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(pagedResult, loopHandling), new DistributedCacheEntryOptions
        //{
        //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),

        //}, token: cancellationToken);

        return new PagedList<NewsResponseModel>(mapperList, (int)totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
    

    private IQueryable<NewsResponseModel> SortResponse(IQueryable<NewsResponseModel> queryable, NewsQueryFilter filter)
    {
        if (string.IsNullOrEmpty(filter.Sort))
        {
            return queryable; // Default sort by CreatedAt ASC
        }
        var sortField = filter.Sort.ToLower();
        var sortDirection = filter.Direction.ToLower();

        // Apply sorting based on sort field and direction
        switch (sortField)
        {
            case "date":
                return sortDirection == "desc"
                  ? queryable.OrderByDescending(n => n.CreatedAt)
                  : queryable.OrderBy(n => n.CreatedAt);
            case "view":
                return sortDirection == "desc"
                  ? queryable.OrderByDescending(n => n.View)
                  : queryable.OrderBy(n => n.View);
            default:
                // Handle invalid sort criteria (optional: log or throw exception)
                return queryable;
        }
    }
}
