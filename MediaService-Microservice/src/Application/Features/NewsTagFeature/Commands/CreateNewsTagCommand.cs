using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.NewsTagModel;
using Application.Common.UUID;
using Application.Constants;
using Application.Features.NewsFeature.Commands;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;

#pragma warning disable
namespace Application.Features.NewsTagFeature.Commands;

public record CreateNewsTagCommand : IRequest<ResponseModel>
{
    public NewsTagCreateRequestModel NewsTagCreateRequestModel;
}

public class CreateNewsTagCommandHandler(MediaDbContext dbContext, IMapper mapper,
    IProducerService producer,
    IClaimInterface claimInterface, ILogger<NewsFileCreateCommand> logger)
    : IRequestHandler<CreateNewsTagCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateNewsTagCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var test = dbContext.NewsTags
                .Find(name => name.NewTagName.Equals(request.NewsTagCreateRequestModel.NewTagName)).FirstOrDefault(cancellationToken);
            if (test != null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NewTagExist);
            }
            var user = claimInterface.GetCurrentUserId;
            var tag = mapper.Map<NewsTag>(request.NewsTagCreateRequestModel);
            tag.Id = new UuidV7().Value;
            tag.CreatedBy = user;
            tag.CreatedAt = DateTime.UtcNow;
            await dbContext.NewsTags.InsertOneAsync(tag);

            return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CreateSuccess, tag.Id);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex.Message);
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CreateFail);
        }
    }
}
