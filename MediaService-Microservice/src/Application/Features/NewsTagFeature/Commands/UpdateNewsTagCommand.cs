using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.NewsTagModel;
using Application.Constants;
using Application.Features.NewsFeature.Commands;
using Domain.Entities;
using Domain.Entities.SqlEntites;
using Infrastructure.Data;
using MongoDB.Driver;
#pragma warning disable
namespace Application.Features.NewsTagFeature.Commands;

public record UpdateNewsTagCommand : IRequest<ResponseModel>
{
    public NewsTagUpdateRequestModel NewsTagUpdateRequestModel;
    public Guid Id { get; set; }
}
public class UpdateNewsTagCommandHandler(MediaDbContext dbContext, IMapper mapper,
    IClaimInterface claimInterface, ILogger<UpdateNewsTagCommand> logger)
    : IRequestHandler<UpdateNewsTagCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(UpdateNewsTagCommand request, CancellationToken cancellationToken)
    {
        var tag = dbContext.NewsTags
                .Find(name => name.Id.Equals(request.Id)).FirstOrDefault(cancellationToken);
        if (tag == null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NotFound + "danh mục tin tức" + request.Id);
        }
        var check = dbContext.NewsTags
                .Find(name => name.NewTagName.Equals(tag.NewTagName)).FirstOrDefault(cancellationToken);
        if (check != null)
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NewTagExist);
        }
        var user = claimInterface.GetCurrentUserId;
        tag.NewTagName = tag.NewTagName == request.NewsTagUpdateRequestModel.NewTagName ? tag.NewTagName : request.NewsTagUpdateRequestModel.NewTagName;
        tag.UpdatedAt = DateTime.Now;
        tag.UpdatedBy = user;
        var filter = Builders<NewsTag>.Filter.Eq(n => n.Id, request.Id);
        var update = Builders<NewsTag>.Update
            .Set(n => n.NewTagName, tag.NewTagName)
            .Set(n => n.UpdatedBy, tag.UpdatedBy)
            .Set(n => n.UpdatedAt, tag.UpdatedAt);

        var result = await dbContext.NewsTags.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        if (result.ModifiedCount > 0)
        {
            return new ResponseModel(HttpStatusCode.OK, ResponseConstaints.UpdateSuccessful);
        }
        else
        {
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.UpdateFail);
        }
    }
}
