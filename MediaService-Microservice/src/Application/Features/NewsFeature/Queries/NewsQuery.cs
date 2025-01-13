using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Amazon.S3.Model;
using Application.Common.Models.NewsModel;
using Application.Services.CacheService.Interfaces;
using Confluent.Kafka;
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
    UserServiceRpc.UserServiceRpcClient client,
    IMapper mapper, 
    IOptions<PaginationOptions> paginationOptions, 
    IRedisDistributedCache redisCache) : IRequestHandler<NewsQuery, PagedList<NewsResponseModel>>
{

    public async Task<PagedList<NewsResponseModel>> Handle(NewsQuery request, CancellationToken cancellationToken)
    {
        // Set default values
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? paginationOptions.Value.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? paginationOptions.Value.DefaultPageSize : request.QueryFilter.PageSize;
        request.QueryFilter.Search = request.QueryFilter.Search == null ? string.Empty : request.QueryFilter.Search;
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


        var result = dbContext.News.Find(n => n.NewName.ToLower().Contains(normalInput) && n.IsDeleted == false).ToList().AsQueryable();
        if (!result.Any())
        {
            return new PagedList<NewsResponseModel>(new List<NewsResponseModel>(), 0, 0, 0);
        }
        // Filter by NewsTagId (if provided)
        if (!string.IsNullOrEmpty(request.QueryFilter.NewsTagId.ToString()))
        {
            result = result.Where(n => n.NewsTagId == request.QueryFilter.NewsTagId);
        }

        // Filter by Location (if provided)
        if (!string.IsNullOrEmpty(request.QueryFilter.Location))
        {
            result = result.Where(n => n.Location.ToLower().Contains(request.QueryFilter.Location.ToLower())); // Adjust comparison as needed (e.g., exact match)
        }

        //grpc
        UserMediaRequest userRequest = new() { UserId = "" };
        var subjectGradeResponse = await client.GetUserMediaAsync(userRequest);
        List<Author> test = new List<Author>();
        for (int i = 0; i < subjectGradeResponse.UserId.Count(); i++) 
        {
            Author h = new Author() 
            { 
                AuthorId = Guid.Parse(subjectGradeResponse.UserId[i]),
                AuthorName = subjectGradeResponse.Username[i],
                AuthorImage = subjectGradeResponse.Image[i]
            };
            test.Add(h);
        }

        var newsTagIds = result.Select(n => n.NewsTagId).Distinct().ToList();
        var newsTags = await dbContext.NewsTags
            .Find(Builders<NewsTag>.Filter.In(t => t.Id, newsTagIds)).ToListAsync(cancellationToken: cancellationToken);
        
        var mapperList = result.Select(n => new NewsResponseModel
        {
            Id = n.Id,
            Author = test.FirstOrDefault(x => x.AuthorId.Equals(n.AuthorId)),
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
            NewsTagName = newsTags.FirstOrDefault(t => t.Id == n.NewsTagId).NewTagName,
        }).ToList();
        var news = SortResponse(mapperList.AsQueryable(), request.QueryFilter);
        mapperList = news.Skip((request.QueryFilter.PageNumber - 1) * request.QueryFilter.PageSize)
            .Take(request.QueryFilter.PageSize).ToList();
        var totalcount = result.Count();
        var pagedResult = new PagedList<NewsResponseModel>(mapperList, totalcount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        var loopHandling = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        // Cache the result with a TTL
        //await redisCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(pagedResult, loopHandling), new DistributedCacheEntryOptions
        //{
        //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),

        //}, token: cancellationToken);

        return pagedResult;
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
