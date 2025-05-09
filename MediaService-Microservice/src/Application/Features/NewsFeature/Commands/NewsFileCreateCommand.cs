using System.Net;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.NewsModel;
using Application.Constants;
using Application.Services.CacheService.Interfaces;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Features.NewsFeature.Commands;

public class NewsFileCreateCommand : IRequest<ResponseModel>
{
    public NewsFileUploadRequestModel NewsFileUploadRequestModel;
    public Guid NewsId;
}
public class TheoryFileCreateCommandHandler(
    MediaDbContext context,
    ICloudinaryService cloudinaryService,
    ILogger<NewsFileCreateCommand> logger,
    IRedisDistributedCache redisDistributedCache)
    : IRequestHandler<NewsFileCreateCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(NewsFileCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var test = context.News
                    .Find(name => name.Id.Equals(request.NewsId)).FirstOrDefault(cancellationToken);
            if (test != null)
            {
                var result = await cloudinaryService.UploadAsync(request.NewsFileUploadRequestModel.ImageFiles);
                if (result.Error != null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, result.Error.Message);
                }
                NewsFile news = new()
                {
                    NewsId = request.NewsId,
                    UpdatedAt = DateTime.UtcNow,
                    Id = ObjectId.GenerateNewId(),
                    CreatedAt = DateTime.Now,
                    FileType = result.ResourceType,
                    File = result.Url.ToString(),
                    FileExtention = result.Format,
                    PublicId = result.PublicId
                };
                await context.NewsFiles.InsertOneAsync(news, cancellationToken: cancellationToken);
                await ClearRelatedCache();
                return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CreateSuccess, news.File);
            }
            else 
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NotFound);
            }
           
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CreateFail);
        }
    }
    private async Task ClearRelatedCache()
    {
        var cachePatterns = new[] { "news:id:*", "news:tag:*", "news:*" };

        foreach (var pattern in cachePatterns)
        {
            await foreach (var key in redisDistributedCache.ScanAsync(pattern))
            {
                await redisDistributedCache.RemoveAsync(key);
            }
        }
    }
}
