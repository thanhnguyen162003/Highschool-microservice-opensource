using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardFolderModel;
using CloudinaryDotNet.Actions;
using Confluent.Kafka;
using Dapr.Client;
using Domain.CustomEntities;
using Domain.DaprModels;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Application.Features.FlashcardFolderUserFeature.Queries
{
    public class FolderUserQuery : IRequest<PagedList<ExisItemFolderUserResponse>>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? Username { get; set; }
        public Guid? FlashcardId { get; set; }
        public Guid? DocumentId { get; set; }
    }

    public class FolderQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claim, DaprClient client) : IRequestHandler<FolderUserQuery, PagedList<ExisItemFolderUserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IClaimInterface _claim = claim;
        private readonly DaprClient _client = client;

        public async Task<PagedList<ExisItemFolderUserResponse>> Handle(FolderUserQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<FolderUser> result = new List<FolderUser>();
            var isMine = false;

            if (!request.Username.IsNullOrEmpty())
            {
                var response = await _client.InvokeMethodAsync<UserResponseDapr>(
                    HttpMethod.Get,
                    "user-sidecar",
                    $"api/v1/dapr/user?username={request.Username}",
                    cancellationToken
                );

                if (_claim.IsAuthenticated && _claim.GetCurrentUserId.ToString().Equals(response.UserId))
                {
                    result = await _unitOfWork.FolderUserRepository.GetMyFolder(new Guid(response.UserId));
                    isMine = true;
                } else if (_claim.IsAuthenticated && !_claim.GetCurrentUserId.ToString().Equals(response.UserId))
                {
                    result = await _unitOfWork.FolderUserRepository.GetFolderByUserId(new Guid(response.UserId));
                } else 
                {
                    result = await _unitOfWork.FolderUserRepository.GetFolderByUserId(new Guid(response.UserId));
                }
            } else
            {
                if (_claim.IsAuthenticated)
                {
                    result = await _unitOfWork.FolderUserRepository.GetMyFolder(_claim.GetCurrentUserId);
                    isMine = true;
                } else
                {
                    result = await _unitOfWork.FolderUserRepository.GetFolderUsers();
                }
            }

            var list = _mapper.Map<IEnumerable<ExisItemFolderUserResponse>>(result, opts =>
            {
                opts.Items["FlashcardId"] = request.FlashcardId;
                opts.Items["DocumentId"] = request.DocumentId;
                opts.Items["IsMine"] = isMine;
            });

            if (request.PageSize == -1)
            {
                return PagedList<ExisItemFolderUserResponse>.Create(list, 1, list.Count());
            }
		
			return PagedList<ExisItemFolderUserResponse>.Create(list.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize), request.PageNumber, request.PageSize);
        }
    }
}
