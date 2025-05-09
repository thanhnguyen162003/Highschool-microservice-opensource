using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FSRSPresetModel;
using Application.Common.Models.NewsModel;
using Dapr.Client;
using Domain.CustomEntities;
using Domain.DaprModels;
using Domain.Entities;
using Domain.Enums;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;


namespace Application.Features.FSRSPresetFeature.Queries;

public record FSRSPresetQuery : IRequest<PagedList<FSRSPresetResponseModel>>
{
    public Guid UserId { get; set; }
    public FSRSPresetQueryFilter QueryFilter { get; set;}
}

public class FSRSPresetQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
    IClaimInterface claimInterface, DaprClient client)
    : IRequestHandler<FSRSPresetQuery, PagedList<FSRSPresetResponseModel>>
{

    public async Task<PagedList<FSRSPresetResponseModel>> Handle(FSRSPresetQuery request, CancellationToken cancellationToken)
    {
        var userId = claimInterface.GetCurrentUserId;
        
        var role = claimInterface.GetRole;
        List<FSRSPreset> list = new();
        int totalCount = 0;
        if (role == RoleEnum.Admin.ToString() || role == RoleEnum.Moderator.ToString())
        {
            (list, totalCount) = await unitOfWork.FSRSPresetRepository.GetPresetAsyncAdmin(request.QueryFilter);
        }
        else
        {
            (list, totalCount) = await unitOfWork.FSRSPresetRepository.GetPresetAsync(request.QueryFilter, request.UserId);
        }

        if (!list.Any())
        {
            return new PagedList<FSRSPresetResponseModel>(new List<FSRSPresetResponseModel>(), 0, 0, 0);
        }

        var mappedList = mapper.Map<List<FSRSPresetResponseModel>>(list);
        var userList = list.Select(x => x.UserId).Distinct().ToList();
		foreach (var item in userList)
        {
			var response = await client.InvokeMethodAsync<UserResponseDapr>(
			        HttpMethod.Get,
			        "user-sidecar",
					$"api/v1/dapr/user?username={item.ToString()}",
					cancellationToken
				);
            Author author = new()
            {
                AuthorId = userId,
                AuthorName = response.Fullname,
                AuthorImage = response.Avatar,
            };
            mappedList
                .Where(x => x.Author.AuthorId == item)
                .ToList()
                .ForEach(x =>
                {
                    x.Author = author;
                });
        }
        

        
       
        return new PagedList<FSRSPresetResponseModel>(mappedList, totalCount, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }

}