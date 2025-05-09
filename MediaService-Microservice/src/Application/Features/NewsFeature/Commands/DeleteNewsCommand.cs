using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.NewsModel;
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

public record DeleteNewsCommand : IRequest<ResponseModel>
{
    public Guid Id { get; init; }
}
public class DeleteNewsCommandHandler(MediaDbContext dbContext, IMapper mapper,
    IClaimInterface claimInterface, IRedisDistributedCache redisCache,
    ICloudinaryService cloudinaryService)
    : IRequestHandler<DeleteNewsCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        var news = dbContext.News
            .Find(name => name.Id.Equals(request.Id)).FirstOrDefault(cancellationToken);
        
        if (news == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NotFound + "tin tức " + request.Id);
        }

        news.DeletedAt = DateTime.UtcNow;
        
        var filter = Builders<News>.Filter.Eq(n => n.Id, request.Id);
        var update = Builders<News>.Update
            .Set(n => n.DeletedAt, news.DeletedAt)
            .Set(n => n.IsDeleted, true);
        var result = await dbContext.News.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        if (result.ModifiedCount > 0)
        {
            var cacheKeyPattern = $"news:*:{request.Id}";
            await ClearRelatedCache();

            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.DeleteSuccess);
        }
        else
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.DeleteFail);
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
