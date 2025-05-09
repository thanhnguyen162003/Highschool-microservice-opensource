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

public record PopularNewsQuery : IRequest<List<NewsResponseModel>>
{
    
}

public class PopularNewsQueryQueryHandler(MediaDbContext dbContext,
    IMapper mapper, IOptions<PaginationOptions> paginationOptions,
    DaprClient daprClient,
    IRedisDistributedCache redisCache) : IRequestHandler<PopularNewsQuery, List<NewsResponseModel>>
{

    public async Task<List<NewsResponseModel>> Handle(PopularNewsQuery request, CancellationToken cancellationToken)
    {

        var now = DateTime.UtcNow;
        var dateToNotGetData = now.Subtract(TimeSpan.FromDays(14));
        var extendedDate = dateToNotGetData.AddDays(26);

        // MongoDB Query with Combined Conditions
        var filter = Builders<News>.Filter.Eq(x => x.IsDeleted, false) &
                     (Builders<News>.Filter.Gte(x => x.CreatedAt, dateToNotGetData) |
                      Builders<News>.Filter.Gte(x => x.CreatedAt, extendedDate));

        var newsList = await dbContext.News
            .Find(filter)
            .SortByDescending(x => x.TotalView)
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
