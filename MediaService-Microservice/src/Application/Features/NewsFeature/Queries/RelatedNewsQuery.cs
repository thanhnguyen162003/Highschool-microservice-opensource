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

public record RelatedNewsQuery : IRequest<List<NewsResponseModel>>
{
    public Guid NewsId;
    public NewsRelatedRequestModel model;
}

public class RelatedNewsQueryHandler(MediaDbContext dbContext,
    UserServiceRpc.UserServiceRpcClient client,
    IMapper mapper, IOptions<PaginationOptions> paginationOptions,
    IRedisDistributedCache redisCache) : IRequestHandler<RelatedNewsQuery, List<NewsResponseModel>>
{

    public async Task<List<NewsResponseModel>> Handle(RelatedNewsQuery request, CancellationToken cancellationToken)
    {
        var result = await dbContext.News.Find(n => n.Location.Trim().ToLower().Contains(request.model.Location.Trim().ToLower())
        && !n.Id.Equals(request.NewsId)).SortByDescending(x => x.CreatedAt).ToListAsync();
        result = result.Where(n => n.NewsTagId.Equals(request.model.NewsTagId)).Take(4).ToList();
        if (!result.Any())
        {
            return new PagedList<NewsResponseModel>(new List<NewsResponseModel>(), 0, 0, 0);
        }
        if (result.Count() < 4)
        {
            var list = await dbContext.News.Find(n => n.NewsTagId.Equals(request.model.NewsTagId)
            && !n.Location.Trim().ToLower().Contains(request.model.Location.Trim().ToLower())
            && !n.Id.Equals(request.NewsId)
            ).SortByDescending(x => x.CreatedAt).ToListAsync();
            result.AddRange(list.Take(4 - result.Count()));
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
            Author= test.FirstOrDefault(x => x.AuthorId.Equals(n.AuthorId)),
            NewsTagId = n.NewsTagId,
            Content = n.Content,
            ContentHtml = n.ContentHtml,
            CreatedAt = n.CreatedAt,
            CreatedBy = n.CreatedBy,
            Image = n.Image,
            IsDeleted = n.IsDeleted,
            NewName = n.NewName,
            Slug = n.Slug,
            UpdatedAt = n.UpdatedAt.Value,
            UpdatedBy = n.UpdatedBy.Value,
            View = n.TotalView,
            Location = n.Location,
            Hot = n.Hot,
            NewsTagName = newsTags.FirstOrDefault(t => t.Id == n.NewsTagId).NewTagName,
        }).ToList();

        var pagedResult = mapper.Map<List<NewsResponseModel>>(mapperList);      

        return pagedResult;
    }

    
}
