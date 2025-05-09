using System.Linq;
using System.Text.RegularExpressions;
using Amazon.S3.Model;
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
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

#pragma warning disable
namespace Application.Features.NewsFeature.Queries;

public record RelatedNewsQuery : IRequest<List<NewsResponseModel>>
{
    public Guid NewsId;
    public NewsRelatedRequestModel model;
}

public class RelatedNewsQueryHandler(MediaDbContext dbContext,
    IMapper mapper, IOptions<PaginationOptions> paginationOptions,
    DaprClient daprClient,
    IRedisDistributedCache redisCache) : IRequestHandler<RelatedNewsQuery, List<NewsResponseModel>>
{

    public async Task<List<NewsResponseModel>> Handle(RelatedNewsQuery request, CancellationToken cancellationToken)
    {
        var locationFilter = new BsonRegularExpression(request.model.Location.Trim(), "i");

        // Combined MongoDB Query with Case-Insensitive Search
        var filter = Builders<News>.Filter.And(
            Builders<News>.Filter.Ne(n => n.Id, request.NewsId),
            Builders<News>.Filter.Eq(n => n.NewsTagId, request.model.NewsTagId),
            Builders<News>.Filter.Eq(n => n.IsDeleted, false),
            Builders<News>.Filter.Or(
                Builders<News>.Filter.Regex(n => n.Location, locationFilter),
                Builders<News>.Filter.Not(Builders<News>.Filter.Regex(n => n.Location, locationFilter))
            )
        );

        // Apply Sorting & Pagination in MongoDB
        var newsList = await dbContext.News
            .Find(filter)
            .SortByDescending(n => n.CreatedAt)
            .Limit(4)
            .ToListAsync(cancellationToken);

        if (!newsList.Any())
        {
            return new List<NewsResponseModel>(); // Return empty list if no data
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
            NewName = n.NewName,
            Location = n.Location,
            Slug = n.Slug,
            UpdatedAt = n.UpdatedAt.GetValueOrDefault(),
            UpdatedBy = n.UpdatedBy.GetValueOrDefault(),
            View = n.TotalView,
            Hot = n.Hot,
            NewsTagName = newsTags.GetValueOrDefault(n.NewsTagId, "Unknown"),
        }).ToList();

        return responseList;
    }

    
}
