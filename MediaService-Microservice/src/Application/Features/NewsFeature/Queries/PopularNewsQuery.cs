using System.Linq;
using System.Text.RegularExpressions;
using Amazon.S3.Model;
using Application.Common.Models.NewsModel;
using Application.Services.CacheService.Interfaces;
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
    UserServiceRpc.UserServiceRpcClient client,
    IMapper mapper, IOptions<PaginationOptions> paginationOptions,
    IRedisDistributedCache redisCache) : IRequestHandler<PopularNewsQuery, List<NewsResponseModel>>
{

    public async Task<List<NewsResponseModel>> Handle(PopularNewsQuery request, CancellationToken cancellationToken)
    {

        var now = DateTime.UtcNow;
        var dateToNotGetData = now.Subtract(TimeSpan.FromDays(14));

        var result = dbContext.News.Find(x => x.IsDeleted == false && x.CreatedAt >= dateToNotGetData).SortByDescending(x => x.TotalView)
        .ToList().Take(4);
        
        if (!result.Any())
        {
            return new PagedList<NewsResponseModel>(new List<NewsResponseModel>(), 0, 0, 0);
        }
        if (result.Count() < 4)
        {
           
            result = dbContext.News.Find(x => x.IsDeleted == false && x.CreatedAt >= dateToNotGetData.AddDays(26)).SortByDescending(x => x.TotalView).ToEnumerable();
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
            Hot = n.Hot,
            NewsTagName = newsTags.FirstOrDefault(t => t.Id == n.NewsTagId).NewTagName,
        }).ToList();

        var pagedResult = mapper.Map<List<NewsResponseModel>>(mapperList);      

        return pagedResult;
    }

    
}
