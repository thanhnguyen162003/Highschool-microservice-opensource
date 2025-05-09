using Algolia.Search.Models.Search;
using Application.Common.UoW;
using Domain.CustomEntities;
using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Application.Common.Helpers;
using Dapr.Client;
using CloudinaryDotNet.Actions;
using Confluent.Kafka;
using Domain.DaprModels;

namespace Application.Features.FlashcardFeature.Queries;

public record FlashcardQueryUser : IRequest<PagedList<FlashcardModel>>
{
    public required FlashcardQueryFilter QueryFilter;
    public required string Username;
}

public class FlashcardQueryUserHandler(
    IUnitOfWork unitOfWork,
    DaprClient client,
    IMapper mapper,
    IOptions<PaginationOptions> paginationOptions)
    : IRequestHandler<FlashcardQueryUser, PagedList<FlashcardModel>>
{
    private readonly PaginationOptions _paginationOptions = paginationOptions.Value;

    public async Task<PagedList<FlashcardModel>> Handle(FlashcardQueryUser request, CancellationToken cancellationToken)
    {
        List<string>? tagsList = request.QueryFilter.Tags?.ToList();
		var response = await client.InvokeMethodAsync<UserResponseDapr>(
		            HttpMethod.Get,
		            "user-sidecar",
					$"api/v1/dapr/user?username={request.Username}",
					cancellationToken
				);
        var listFlashcard = await unitOfWork.FlashcardRepository.GetFlashcardsByUserId(request.QueryFilter, new Guid(response.UserId));
        
        // Sử dụng FlashcardHelper để lọc dựa trên các thuộc tính
        if (listFlashcard.Any())
        {
            listFlashcard = FlashcardHelper.FilterByRelationships(
                listFlashcard,
                request.QueryFilter.EntityId,
                request.QueryFilter.FlashcardType
            );
        }

        if (!listFlashcard.Any())
        {
            return new PagedList<FlashcardModel>(new List<FlashcardModel>(), 0, 0, 0);
        }

        var mapperList = mapper.Map<List<FlashcardModel>>(listFlashcard);

        // Đảm bảo tất cả flashcard đều có tags
        foreach (var flashcard in mapperList)
        {
            if (flashcard.Tags == null || !flashcard.Tags.Any())
            {
                var tags = await unitOfWork.TagRepository.GetTagsByFlashcardIdAsync(flashcard.Id, cancellationToken);
                if (tags != null && tags.Any())
                {
                    flashcard.Tags = tags.Select(t => t.Name).ToList();
                }
            }
            
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
        
        // Sử dụng helper để tải thông tin liên quan cho tất cả flashcard
        await FlashcardHelper.LoadRelatedEntitiesForList(mapperList, unitOfWork);

        return PagedList<FlashcardModel>.Create(mapperList, request.QueryFilter.PageNumber, request.QueryFilter.PageSize);
    }
}