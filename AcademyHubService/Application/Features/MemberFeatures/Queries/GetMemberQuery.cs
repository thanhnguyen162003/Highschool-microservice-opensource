using Application.Common.Models.OtherModel;
using Application.Common.Models.ZoneMembershipModel;
using AutoMapper;
using Dapr.Client;
using Domain.DaprModels;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using static Domain.Enums.ZoneEnums;

namespace Application.Features.MemberFeatures.Queries
{
    public class GetMemberQuery : IRequest<object>
    {
        public MemberQueryType Type { get; set; }
        public Guid ZoneId { get; set; }
    }

    public class GetMemberQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        DaprClient daprClient, ILogger<GetMemberQueryHandler> logger) : IRequestHandler<GetMemberQuery, object>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly DaprClient _daprClient = daprClient;
        private readonly IMapper _mapper = mapper;
		private readonly ILogger<GetMemberQueryHandler> _logger = logger;

        public async Task<object> Handle(GetMemberQuery request, CancellationToken cancellationToken)
		{
			if (request.Type == MemberQueryType.Member)
			{
				return await GetMembers(request.ZoneId, cancellationToken);
			}
			else if (request.Type == MemberQueryType.Pending)
			{
				return await GetPendingMembers(request.ZoneId, cancellationToken);
			} else if(request.Type == MemberQueryType.All)
			{
				return new
				{
					Members = await GetMembers(request.ZoneId, cancellationToken),
					PendingMembers = await GetPendingMembers(request.ZoneId, cancellationToken)
				};
			}

			return Enumerable.Empty<object>();
		}

		private async Task<IEnumerable<MemberZoneResponseModel>> GetMembers(Guid zoneId, CancellationToken cancellationToken)
		{
			var members = await _unitOfWork.ZoneMembershipRepository.GetAll(m => m.ZoneId.Equals(zoneId));
			var zone = await _unitOfWork.ZoneRepository.GetById(zoneId);

            var memberResponse = _mapper.Map<IEnumerable<MemberZoneResponseModel>>(members);
            memberResponse = memberResponse.Prepend(new MemberZoneResponseModel()
			{
				CreatedAt = zone?.CreatedAt,
				Id = 0,
                Role = ZoneMembershipType.Owner.ToString(),
				Email = zone?.CreatedBy.ToString()!,
            });

            if (!memberResponse.Any())
			{
				return memberResponse;
			}

			foreach (var member in memberResponse)
			{
				var response = await _daprClient.InvokeMethodAsync<UserResponseDapr>(
					HttpMethod.Get,
					"user-sidecar",
					$"api/v1/dapr/user?username={Uri.EscapeDataString(member.Email)}",
					cancellationToken
				);

                member.User = _mapper.Map<UserModel>(response);
				member.Email = response.Email ?? member.Email;
            }

            return memberResponse;
		}

		private async Task<IEnumerable<MemberPendingResponseModel>> GetPendingMembers(Guid zoneId, CancellationToken cancellationToken)
		{
			var members = await _unitOfWork.PendingZoneInviteRepository.GetAll(m => m.ZoneId.Equals(zoneId));

			var memberResponse = _mapper.Map<IEnumerable<MemberPendingResponseModel>>(members);

			if (members == null || !members.Any())
			{
				return memberResponse;
			}

			foreach (var member in memberResponse)
			{
				var response = await _daprClient.InvokeMethodAsync<UserResponseDapr>(
					HttpMethod.Get,
					"user-sidecar",
					$"api/v1/dapr/user?username={Uri.EscapeDataString(member.Email)}",
					cancellationToken
				);

				member.User = _mapper.Map<UserModel>(response);
			}

			return memberResponse;
		}
	}

}
