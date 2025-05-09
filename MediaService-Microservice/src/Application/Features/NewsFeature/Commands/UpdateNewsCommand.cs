using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.DaprModel.User;
using Application.Common.Models.NewsModel;
using Application.Constants;
using Application.Services;
using Application.Services.CacheService.Interfaces;
using Dapr.Client;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
#pragma warning disable
namespace Application.Features.NewsFeature.Commands;

public record UpdateNewsCommand : IRequest<ResponseModel>
{
    public NewsUpdateRequestModel NewsUpdateRequestModel;
    public Guid Id { get; init; }
}
public class UpdateNewsCommandHandler(MediaDbContext dbContext, IMapper mapper,
    DaprClient daprClient,
    IClaimInterface claimInterface, IRedisDistributedCache redisCache,
    ICloudinaryService cloudinaryService)
    : IRequestHandler<UpdateNewsCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        var news = dbContext.News
            .Find(name => name.Id.Equals(request.Id)).FirstOrDefault(cancellationToken);
        
        if (news == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NotFound + "tin tức " + request.Id);
        }

        // gRPC Call to Fetch Authors
        var subjectGradeResponse = await daprClient.InvokeMethodAsync<UserResponseMediaDapr>(
                            HttpMethod.Get,
                            "user-sidecar",
                            $"api/v1/dapr/user-media?userId={userId.ToString()}"
                        );
        var authorLookup = new Author
        {
            AuthorId = Guid.Parse(subjectGradeResponse.UserId[0]),
            Avatar = subjectGradeResponse.Avatar[0],
            FullName = subjectGradeResponse.Username[0]
        };
        if (authorLookup is null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, "Không tìm thấy người dùng");
        }
        if (request.NewsUpdateRequestModel.Hot)
        {
            var hotNews = await dbContext.News.Find(n => n.Hot == true).ToListAsync();
            if (hotNews.Count() > 0)
            {
                if (!hotNews.FirstOrDefault().Id.Equals(request.Id))
                {
                    var response = new MarkHotNewResponseModel()
                    {
                        NewId = hotNews.FirstOrDefault().Id,
                        NewName = hotNews.FirstOrDefault().NewName
                    };
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.HotNewExist, response);
                }
            }
        }

        var check = dbContext.NewsTags
            .Find(name => name.Id.Equals(request.NewsUpdateRequestModel.NewsTagId)).FirstOrDefault(cancellationToken);
        if (check == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NotFound + "danh mục tin tức " + request.NewsUpdateRequestModel.NewsTagId);
        }
        news.Author = authorLookup;
        news.NewsTagId = request.NewsUpdateRequestModel.NewsTagId;
        news.ContentHtml = request.NewsUpdateRequestModel.ContentHtml;
        news.Content = request.NewsUpdateRequestModel.Content;
        news.Location = request.NewsUpdateRequestModel.Location.Trim().ToLower();
        news.Hot = request.NewsUpdateRequestModel.Hot;
        news.UpdatedAt = DateTime.UtcNow;
        news.UpdatedBy = userId;
        
        var filter = Builders<News>.Filter.Eq(n => n.Id, request.Id);
        var update = Builders<News>.Update
            .Set(n => n.Author, news.Author)
            .Set(n => n.NewsTagId, news.NewsTagId)
            .Set(n => n.NewName, news.NewName)
            .Set(n => n.ContentHtml, news.ContentHtml)
            .Set(n => n.Content, news.Content)
            .Set(n => n.Slug, news.Slug)
            .Set(n => n.Image, news.Image)
            .Set(n => n.TodayView, news.TodayView)
            .Set(n => n.TotalView, news.TotalView)
            .Set(n => n.Location, news.Location)
            .Set(n => n.UpdatedAt, DateTime.UtcNow)
            .Set(n => n.Hot, news.Hot)
            .Set(n => n.UpdatedBy, news.UpdatedBy);

        var result = await dbContext.News.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        if (result.ModifiedCount > 0)
        {
            var cacheKeyPattern = $"news:*:{request.Id}";
            await ClearRelatedCache();

            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.UpdateSuccessful);
        }
        else
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.UpdateFail);
        }
    }
    
    private async Task ClearRelatedCache()
    {
        var cachePatterns = new[] { "news:id:*", "news:tag:*", "news:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in redisCache.ScanAsync(pattern))
            {
                await redisCache.RemoveAsync(key);
            }
        }
    }
}
