using Algolia.Search.Models.Search;
using Application.Common.UoW;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Features.FlashcardFeature.Queries;

public record FlashcardQueryUser : IRequest<PagedList<FlashcardModel>>
{
    public FlashcardQueryFilter QueryFilter;
    public string Username;
}

public class FlashcardQueryUserHandler(
    IUnitOfWork unitOfWork,
    UserServiceRpc.UserServiceRpcClient client,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<FlashcardQueryUser, PagedList<FlashcardModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<FlashcardModel>> Handle(FlashcardQueryUser request, CancellationToken cancellationToken)
    {
        GetUserRequest requestRpc = new() {Username = request.Username};
        UserResponse responseRpc = await client.GetUserAsync(requestRpc);
        var listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcardsByUserId(request.QueryFilter, new Guid(responseRpc.UserId));
        
        if (!listFlashcard.Any())
        {
            return new PagedList<FlashcardModel>(new List<FlashcardModel>(), 0, 0, 0);
        }
        
        var mapperList = mapper.Map<List<FlashcardModel>>(listFlashcard);
        foreach (var flashcard in mapperList)
        {
            var userLike = await unitOfWork.UserLikeRepository.GetUserLikeFlashcardAsync(flashcard.UserId, flashcard.Id);
            if (userLike != null)
            {
                if (userLike.FlashcardId == flashcard.Id)
                {
                    flashcard.IsRated = true;
                }
                else
                {
                    flashcard.IsRated = false;
                }
            }
        }
        return PagedList<FlashcardModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}