using System.Collections.Generic;
using Application.Common.Models.NewsTagModel;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Domain.QueriesFilter;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
#pragma warning disable
namespace Application.Features.NewsTagFeature.Queries;

public record NewsTagQuery : IRequest<PagedList<NewsTagResponseModel>>
{
    public NewsTagQueryFilter QueryFilter;
}

public class NewsTagQueryHandler(MediaDbContext dbContext, IMapper mapper, IOptions<PaginationOptions> paginationOptions) : IRequestHandler<NewsTagQuery, PagedList<NewsTagResponseModel>>
{
    public async Task<PagedList<NewsTagResponseModel>> Handle(NewsTagQuery request, CancellationToken cancellationToken)
    {
        request.QueryFilter.PageSize = request.QueryFilter.PageSize == 0 ? paginationOptions.Value.DefaultPageSize : request.QueryFilter.PageSize;
        request.QueryFilter.Search = request.QueryFilter.Search == null ? "" : request.QueryFilter.Search;

        var filter = Builders<NewsTag>.Filter.Regex(n => n.NewTagName, new BsonRegularExpression(request.QueryFilter.Search, "i"));
        var sort = Builders<NewsTag>.Sort.Descending(n => n.CreatedAt);
        var options = new FindOptions<NewsTag, object>
        {
            Sort = sort
        };
        if (request.QueryFilter.PageNumber == -1)
        {
            var list = await dbContext.NewsTags.FindAsync(filter, options, cancellationToken);
            var result = list.ToList();
            if (!result.Any())
            {
                return new PagedList<NewsTagResponseModel>(new List<NewsTagResponseModel>(), 0, 0, 0);
            }
            var mapperList = mapper.Map<List<NewsTagResponseModel>>(result);
            return PagedList<NewsTagResponseModel>.Create(mapperList, 1, result.Count());
        }
        else
        {
            request.QueryFilter.PageNumber = request.QueryFilter.PageNumber == 0 ? paginationOptions.Value.DefaultPageNumber : request.QueryFilter.PageNumber;
            var listChapter = await dbContext.NewsTags.FindAsync(filter, options, cancellationToken);
            var result = listChapter.ToList();
            if (!result.Any())
            {
                return new PagedList<NewsTagResponseModel>(new List<NewsTagResponseModel>(), 0, 0, 0);
            }
            var mapperList = mapper.Map<List<NewsTagResponseModel>>(result);
            return PagedList<NewsTagResponseModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
        }
        
    }
}
