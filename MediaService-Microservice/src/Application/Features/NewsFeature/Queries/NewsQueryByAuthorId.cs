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

public record NewsQueryByAuthorId: IRequest<PagedList<NewsResponseModel>>
{
    public Guid AuthorId;
    public NewsAuthorQueryFilter QueryFilter;
}

public class NewsQueryByAuthorIdHandler(MediaDbContext dbContext, IMapper mapper,
    UserServiceRpc.UserServiceRpcClient client,
    IOptions<PaginationOptions> paginationOptions, 
    IRedisDistributedCache redisCache) 
    : IRequestHandler<NewsQueryByAuthorId, PagedList<NewsResponseModel>>
{ 
    public async Task<PagedList<NewsResponseModel>> Handle(NewsQueryByAuthorId request, CancellationToken cancellationToken)
    {
        request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? paginationOptions.Value.DefaultPageNumber : request.QueryFilter.PageNumber;
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? paginationOptions.Value.DefaultPageSize : request.QueryFilter.PageSize;
        request.QueryFilter.Search = request.QueryFilter.Search == null ? string.Empty : request.QueryFilter.Search;
        var normalInput = Regex.Replace(request.QueryFilter.Search.Trim(), @"\s+", " ").ToLower();

        var result = await dbContext.News.Find(n => n.AuthorId.Equals(request.AuthorId) && n.IsDeleted == false && n.NewName.ToLower().Contains(normalInput)).ToListAsync(cancellationToken: cancellationToken);
        if (!result.Any())
        {
            return new PagedList<NewsResponseModel>(new List<NewsResponseModel>(), 0, 0, 0);
        }
        switch (request.QueryFilter.Direction)
        {
            case "asc": 
                result = result.OrderBy(n => n.CreatedAt).ToList();
                break;

            case "desc":
                result = result.OrderByDescending(n => n.CreatedAt).ToList();
                break;

            default: break;

        }

        //grpc
        UserMediaRequest userRequest = new() { UserId = request.AuthorId.ToString() };
        var subjectGradeResponse = await client.GetUserMediaAsync(userRequest);
        Author author = new Author();
        if (subjectGradeResponse.UserId.Count != 0
            && subjectGradeResponse.Image.Count != 0
            && subjectGradeResponse.Username.Count != 0)
        {
            author.AuthorId = Guid.Parse(subjectGradeResponse.UserId[0].ToString());
            author.AuthorImage = subjectGradeResponse.Image[0].ToString();
            author.AuthorName = subjectGradeResponse.Username[0].ToString();
        }
        var newsTagIds = result.Select(n => n.NewsTagId).Distinct().ToList();
        var newsTags = await dbContext.NewsTags.Find(Builders<NewsTag>.Filter.In(t => t.Id, newsTagIds)).ToListAsync(cancellationToken);

        var mapperList = result.Select(n => new NewsResponseModel
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
            UpdatedAt = n.UpdatedAt.Value,
            UpdatedBy = n.UpdatedBy.Value,
            View = n.TotalView,
            NewsTagName = newsTags.FirstOrDefault(t => t.Id == n.NewsTagId).NewTagName,
        }).ToList();
        mapperList = mapperList.Skip((request.QueryFilter.PageNumber - 1) * request.QueryFilter.PageSize)
            .Take(request.QueryFilter.PageSize).ToList();
        var totalcount = result.Count();
        var pagedResult = new PagedList<NewsResponseModel>(mapperList, totalcount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);

        return pagedResult;
    }

}
