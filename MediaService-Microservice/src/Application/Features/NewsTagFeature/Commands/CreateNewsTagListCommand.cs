using System.Net;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.CommonModels;
using Application.Common.Models.NewsTagModel;
using Application.Common.UUID;
using Application.Constants;
using Domain.Entities;
using Infrastructure.Data;
using MongoDB.Driver;

#pragma warning disable
namespace Application.Features.NewsTagFeature.Commands;

public record CreateNewsTagListCommand : IRequest<ResponseModel>
{
    public List<NewsTagCreateRequestModel>  NewsTagCreateRequestModel;
}

public class CreateTagListCommandHandler(MediaDbContext dbContext, IMapper mapper,
    IProducerService producer,
    IClaimInterface claimInterface, ILogger<CreateNewsTagListCommand> logger)
    : IRequestHandler<CreateNewsTagListCommand, ResponseModel>
{
    public async Task<ResponseModel> Handle(CreateNewsTagListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = claimInterface.GetCurrentUserId;
            var listTag = mapper.Map<List<NewsTag>>(request.NewsTagCreateRequestModel);
            var tagIds = new List<Guid>();
            var listAdd = new List<NewsTag>();
            foreach (var tag in listTag)
            {
                var test = dbContext.NewsTags
                    .Find(name => name.NewTagName.Equals(tag.NewTagName)).FirstOrDefault(cancellationToken);
                if (test != null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NewTagExist);
                }
                tag.Id = new UuidV7().Value;
                tag.CreatedBy = user;
                tag.CreatedAt = DateTime.UtcNow;
                listAdd.Add(tag);
            }
            if (listAdd.Any())
            {
                // Filter out duplicate tags based on NewTagName
                listAdd = listAdd.GroupBy(tag => tag.NewTagName)
                                                     .Select(group => group.First())
                                                     .ToList();
                foreach (var item in listAdd)
                {
                    tagIds.Add(item.Id);
                }
                await dbContext.NewsTags.InsertManyAsync(listAdd);
                return new ResponseModel(HttpStatusCode.Created, ResponseConstaints.CreateSuccess, tagIds);
            }
            else
            {
                return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.NewTagExist);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return new ResponseModel(HttpStatusCode.BadRequest, ResponseConstaints.CreateFail);
        }
    }
}

