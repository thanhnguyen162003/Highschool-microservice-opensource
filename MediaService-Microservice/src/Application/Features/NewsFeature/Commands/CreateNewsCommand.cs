using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.NewsModel;
using Application.Common.Ultils;
using Application.Common.UUID;
using Application.Constants;
using Application.Services;
using Application.Services.CacheService.Interfaces;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

#pragma warning disable
namespace Application.Features.NewsFeature.Commands;

public record CreateNewsCommand : IRequest<ResponseModel>
{
    public NewsCreateRequestModel NewsCreateRequestModel;
}

public class CreateNewsCommandHandler(MediaDbContext dbContext, IMapper mapper,
    IClaimInterface claimInterface, ILogger<CreateNewsCommand> logger,
    ICloudinaryService cloudinaryService,
    IRedisDistributedCache redisCache)
    : IRequestHandler<CreateNewsCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingNews = dbContext.News
                .Find(name => name.NewName.Equals(request.NewsCreateRequestModel.NewName))
                .FirstOrDefault(cancellationToken);
            if (existingNews != null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NewExist);
            }

            // Check if news tag ID exists
            var tagExists = dbContext.NewsTags
                .Find(tag => tag.Id.Equals(request.NewsCreateRequestModel.NewsTagId))
                .FirstOrDefault(cancellationToken);
            if (tagExists == null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NotFound + "danh mục tin tức " + request.NewsCreateRequestModel.NewsTagId);
            }

            // Initialize new news entry
            var userId = claimInterface.GetCurrentUserId;
            var news = mapper.Map<News>(request.NewsCreateRequestModel);
            news.Id = new UuidV7().Value;
            news.AuthorId = userId;
            news.TodayView = 0;
            news.TotalView = 0;
            news.CreatedBy = userId;
            news.UpdatedBy = userId;
            news.Slug = SlugHelper.GenerateSlug(news.NewName, news.Id.ToString());
            news.CreatedAt = DateTime.UtcNow;
            news.UpdatedAt = DateTime.UtcNow;
            news.IsDeleted = false;
            news.Hot = false;
            var result = await cloudinaryService.UploadAsync(request.NewsCreateRequestModel.Image);
            if (result.Error != null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, result.Error.Message);
            }
            NewsFile newsFile = new()
            {
                NewsId = news.Id,
                UpdatedAt = DateTime.UtcNow,
                Id = ObjectId.GenerateNewId(),
                CreatedAt = DateTime.Now,
                FileType = result.ResourceType,
                File = result.Url.ToString(),
                FileExtention = result.Format,
                PublicId = result.PublicId
            };
            news.Image = newsFile.File;
            //news.Image = request.NewsCreateRequestModel.Image.

            await dbContext.News.InsertOneAsync(news, cancellationToken: cancellationToken);
            await ClearRelatedCache();

            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CreateSuccess, news.Slug);
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
            await foreach (var key in redisCache.ScanAsync(pattern))
            {
                await redisCache.RemoveAsync(key);
            }
        }
    }
}
