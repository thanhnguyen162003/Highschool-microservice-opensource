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

public record HotNewsQuery : IRequest<List<NewsResponseModel>>
{
    
}

public class HotNewsQueryHandler(MediaDbContext dbContext,
    IMapper mapper, IOptions<PaginationOptions> paginationOptions,
    DaprClient daprClient,
    IRedisDistributedCache redisCache) : IRequestHandler<HotNewsQuery, List<NewsResponseModel>>
{

    public async Task<List<NewsResponseModel>> Handle(HotNewsQuery request, CancellationToken cancellationToken)
    {
        List<News> result = new();

        var filter = Builders<News>.Filter.Eq(n => n.IsDeleted, false);
        var hotNews = await dbContext.News.Find(filter & Builders<News>.Filter.Eq(n => n.Hot, true))
                                          .ToListAsync(cancellationToken);

        var list = await dbContext.News.Find(filter & Builders<News>.Filter.Eq(n => n.Hot, false))
                                       .SortByDescending(n => n.TodayView)
                                       .Limit(6)
                                       .ToListAsync(cancellationToken);

        if (list.All(news => news.TodayView == 0))
        {
            list = await dbContext.News.Find(filter)
                                       .SortByDescending(n => n.CreatedAt)
                                       .Limit(6)
                                       .ToListAsync(cancellationToken);
        }

        hotNews.AddRange(list);
        result = hotNews.Any() ? hotNews : list;

        if (!result.Any())
        {
            return new PagedList<NewsResponseModel>(new List<NewsResponseModel>(), 0, 0, 0);
        }


        // Fetch and map NewsTags efficiently
        var newsTagIds = result.Select(n => n.NewsTagId).Distinct().ToList();
        var newsTags = (await dbContext.NewsTags.Find(Builders<NewsTag>.Filter.In(t => t.Id, newsTagIds))
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

        var responseList = result.Select(n => new NewsResponseModel
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
            Slug = n.Slug,
            UpdatedAt = n.UpdatedAt.GetValueOrDefault(),
            UpdatedBy = n.UpdatedBy.GetValueOrDefault(),
            Location = n.Location,
            View = n.TotalView,
            Hot = n.Hot,
            NewsTagName = newsTags.GetValueOrDefault(n.NewsTagId, "Unknown"),
        }).ToList();

        return responseList;
    }
}
