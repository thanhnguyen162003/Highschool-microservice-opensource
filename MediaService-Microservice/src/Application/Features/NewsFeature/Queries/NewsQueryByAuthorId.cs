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

public record NewsQueryByAuthorId: IRequest<PagedList<NewsResponseModel>>
{
    public Guid AuthorId;
    public NewsAuthorQueryFilter QueryFilter;
}

public class NewsQueryByAuthorIdHandler(MediaDbContext dbContext, IMapper mapper,
    IOptions<PaginationOptions> paginationOptions,
    DaprClient daprClient,
    IRedisDistributedCache redisCache) 
    : IRequestHandler<NewsQueryByAuthorId, PagedList<NewsResponseModel>>
{ 
    public async Task<PagedList<NewsResponseModel>> Handle(NewsQueryByAuthorId request, CancellationToken cancellationToken)
    {
        // Set default pagination values
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? paginationOptions.Value.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? paginationOptions.Value.DefaultPageSize : request.QueryFilter.PageSize;
        request.QueryFilter.Search = request.QueryFilter.Search ?? string.Empty;

        var normalInput = Regex.Replace(request.QueryFilter.Search.Trim(), @"\s+", " ").ToLower();

        // MongoDB Query Filters
        var filter = Builders<News>.Filter.Eq(n => n.IsDeleted, false) &
                     Builders<News>.Filter.Eq(n => n.Author.AuthorId, request.AuthorId) &
                     Builders<News>.Filter.Regex(n => n.NewName, new BsonRegularExpression(normalInput, "i"));

        // Sorting (MongoDB Sorting Optimization)
        var sortDefinition = request.QueryFilter.Direction?.ToLower() == "desc"
            ? Builders<News>.Sort.Descending(n => n.CreatedAt)
            : Builders<News>.Sort.Ascending(n => n.CreatedAt);

        // Fetch filtered and sorted news from MongoDB with pagination
        var newsList = await dbContext.News
            .Find(filter)
            .Sort(sortDefinition)
            .Skip((request.QueryFilter.PageNumber - 1) * request.QueryFilter.PageSize)
            .Limit(request.QueryFilter.PageSize)
            .ToListAsync(cancellationToken);

        // Fetch total count for pagination
        var totalCount = await dbContext.News.CountDocumentsAsync(filter);

        if (!newsList.Any())
        {
            return new PagedList<NewsResponseModel>([], 0, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        }

        // gRPC Call to Fetch Author Data
        

        // Fetch NewsTags
        var newsTagIds = newsList.Select(n => n.NewsTagId).Distinct().ToList();
        var newsTags = (await dbContext.NewsTags
                .Find(Builders<NewsTag>.Filter.In(t => t.Id, newsTagIds))
                .ToListAsync(cancellationToken))
            .ToDictionary(t => t.Id, t => t.NewTagName);

        var subjectGradeResponse = await daprClient.InvokeMethodAsync<UserResponseMediaDapr>(
                HttpMethod.Get,
                "user-sidecar",
                $"api/v1/dapr/user-media?userId={request.AuthorId}"
            );

        Author? author = null;
        if (subjectGradeResponse.UserId.Count > 0 &&
            subjectGradeResponse.Avatar.Count > 0 &&
            subjectGradeResponse.Username.Count > 0)
        {
            author = new Author
            {
                AuthorId = Guid.Parse(subjectGradeResponse.UserId[0]),
                Avatar = subjectGradeResponse.Avatar[0],
                FullName = subjectGradeResponse.Username[0]
            };
        }
        if (author == null)
        {
            return new PagedList<NewsResponseModel>(new List<NewsResponseModel>(), 0, 0, 0);
        }

        // Map News to Response Model
        var responseList = newsList.Select(n => new NewsResponseModel
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
            Slug = n.Slug,
            Location = n.Location,
            UpdatedAt = n.UpdatedAt.GetValueOrDefault(),
            UpdatedBy = n.UpdatedBy.GetValueOrDefault(),
            View = n.TotalView,
            NewsTagName = newsTags.GetValueOrDefault(n.NewsTagId, "Unknown"),
        }).ToList();

        return new PagedList<NewsResponseModel>(responseList, (int)totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }

}
