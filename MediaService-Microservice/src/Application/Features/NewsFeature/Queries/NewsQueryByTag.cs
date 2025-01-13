using System.Text.RegularExpressions;
using Application.Common.Models.NewsModel;
using Application.Services.CacheService.Interfaces;
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
    UserServiceRpc.UserServiceRpcClient client,
    IOptions<PaginationOptions> paginationOptions, 
    IRedisDistributedCache redisCache) 
    : IRequestHandler<NewsQueryByTag, PagedList<NewsResponseModel>>
{ 
    public async Task<PagedList<NewsResponseModel>> Handle(NewsQueryByTag request, CancellationToken cancellationToken)
    {
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? paginationOptions.Value.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? paginationOptions.Value.DefaultPageSize : request.QueryFilter.PageSize;
        request.QueryFilter.Search = request.QueryFilter.Search == null ? string.Empty : request.QueryFilter.Search;
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
        var result = dbContext.News.Find(n => n.NewName.ToLower().Contains(normalInput) && n.NewsTagId.Equals(request.NewsTagId) && n.IsDeleted == false)
            .ToList().AsQueryable();
        if (!result.Any())
        {
            return new PagedList<NewsResponseModel>(new List<NewsResponseModel>(), 0, 0, 0);
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
        var newsTags = await dbContext.NewsTags.Find(Builders<NewsTag>.Filter.In(t => t.Id, newsTagIds)).ToListAsync(cancellationToken);

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
            Location = n.Location,
            NewName = n.NewName,
            Slug = n.Slug,
            UpdatedAt = n.UpdatedAt.Value,
            UpdatedBy = n.UpdatedBy.Value,
            View = n.TotalView,
            NewsTagName = newsTags.FirstOrDefault(t => t.Id == n.NewsTagId).NewTagName,
        }).OrderByDescending(x => x.CreatedAt).ToList();
        var pagedResult = new PagedList<NewsResponseModel>(mapperList, result.Count() ,request.QueryFilter.PageNumber, request.QueryFilter.PageSize);

        // Cache the result with a 24-hour expiration
        //var loopHandling = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        //await redisCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(pagedResult, loopHandling),
        //    new DistributedCacheEntryOptions
        //    {
        //        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        //    }, token: cancellationToken);

        return pagedResult;
    }

  
}
