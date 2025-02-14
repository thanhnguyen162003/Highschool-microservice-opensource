using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Models.FlashcardFolderModel;
using Confluent.Kafka;
using Domain.CustomEntities;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

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

    public class FolderQueryHandler : IRequestHandler<FolderUserQuery, PagedList<ExisItemFolderUserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimInterface _claim;
        private readonly UserServiceRpc.UserServiceRpcClient _client;

        public FolderQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IClaimInterface claim, UserServiceRpc.UserServiceRpcClient client)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claim = claim;
            _client = client;
        }

        public async Task<PagedList<ExisItemFolderUserResponse>> Handle(FolderUserQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<FolderUser> result = new List<FolderUser>();

            if(!request.Username.IsNullOrEmpty())
            {
                GetUserRequest requestRpc = new() { Username = request.Username };
                UserResponse responseRpc = await _client.GetUserAsync(requestRpc);

                if(_claim.IsAuthenticated && _claim.GetCurrentUserId.ToString().Equals(responseRpc.UserId))
                {
                    result = await _unitOfWork.FolderUserRepository.GetMyFolder(new Guid(responseRpc.UserId));
                } else
                {
                    result = await _unitOfWork.FolderUserRepository.GetFolderByUserId(new Guid(responseRpc.UserId));
                }
            } else
            {
                result = await _unitOfWork.FolderUserRepository.GetFolderUsers();
            }

            var list = _mapper.Map<IEnumerable<ExisItemFolderUserResponse>>(result, opts =>
            {
                opts.Items["FlashcardId"] = request.FlashcardId;
                opts.Items["DocumentId"] = request.DocumentId;
            });

            if (request.PageSize == -1)
            {
                return PagedList<ExisItemFolderUserResponse>.Create(list, 1, list.Count());
            }
		
			return PagedList<ExisItemFolderUserResponse>.Create(list.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize), request.PageNumber, request.PageSize);
        }
    }
}
