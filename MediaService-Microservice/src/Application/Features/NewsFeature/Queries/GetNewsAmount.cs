using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Amazon.S3.Model;
using Application.Common.Models.CommonModels;
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
using static Application.Features.NewsFeature.Queries.GetNewsAmount;

#pragma warning disable
namespace Application.Features.NewsFeature.Queries;

public record GetNewsAmount : IRequest<NewsAmountResponseModel>
{
    public string NewType { get; set; }
}

public class GetNewsAmountHandler(MediaDbContext dbContext,
    IMapper mapper, IOptions<PaginationOptions> paginationOptions,
    IRedisDistributedCache redisCache) : IRequestHandler<GetNewsAmount, NewsAmountResponseModel>
{

    public async Task<NewsAmountResponseModel> Handle(GetNewsAmount request, CancellationToken cancellationToken)
    {
        if (request.NewType == NewsStatisticType.All.ToString())
        {
            var totalNews = await dbContext.News.CountDocumentsAsync(x => x.IsDeleted == false);
            var increaseNews = await dbContext.News.CountDocumentsAsync(x => x.IsDeleted == false && x.CreatedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0));
            var test = DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month);
            var lastMonthNews = await dbContext.News.CountDocumentsAsync(x => x.IsDeleted == false && x.CreatedAt >= new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1, 0, 0, 0) && x.CreatedAt <= new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, test, 0, 0, 0));
            long percent = 0;
            if (lastMonthNews == 0)
            {
                percent = (increaseNews - lastMonthNews) * 100 / 1;
            }
            else percent = (increaseNews - lastMonthNews) * 100 / lastMonthNews;
            var response = new NewsAmountResponseModel()
                {
                    TotalNews = totalNews,
                    ThisMonthNews = increaseNews,
                    IncreaseNewsPercent = percent
                };

            return response;
        }
        else if (request.NewType == NewsStatisticType.Hot.ToString())
        {
            var totalHotNews = await dbContext.News.CountDocumentsAsync(x => x.Hot == true && x.IsDeleted == false);
            var response = new NewsAmountResponseModel()
            {
                TotalHotNews = totalHotNews
            };
            return response;
        }
        else if (request.NewType == NewsStatisticType.Delete.ToString())
        {
            var totalDeletedNews = await dbContext.News.CountDocumentsAsync(x => x.IsDeleted == true);
            var response = new NewsAmountResponseModel()
            {
                TotalDeletedNews = totalDeletedNews
            };
            return response;
        }
        else
        {
            return new NewsAmountResponseModel();
        }
    }

    
}
