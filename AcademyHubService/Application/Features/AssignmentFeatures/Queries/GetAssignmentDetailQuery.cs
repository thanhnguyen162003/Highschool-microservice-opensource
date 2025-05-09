using Application.Common.Models.AssignmentModel;
using AutoMapper;
using Infrastructure.Repositories;
using MediatR;
using Application.Services.Authentication;
using Domain.Enums;
using static StackExchange.Redis.Role;
using Dapr.Client.Autogen.Grpc.v1;
using Dapr.Client;
using Domain.DaprModels;
using Application.Common.Models.ZoneModel;

namespace Application.Features.AssignmentFeatures.Queries
{
   
    public class GetAssignmentDetailQuery : IRequest<AssignmentDetailResponseModel>
    {
        public Guid AssignmentId { get; set; }
    }

    public class GetAssignmentDetailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService claimInterface, DaprClient dapr) : IRequestHandler<GetAssignmentDetailQuery, AssignmentDetailResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthenticationService _claimInterface = claimInterface;
        private readonly DaprClient _dapr = dapr;

        public async Task<AssignmentDetailResponseModel> Handle(GetAssignmentDetailQuery request, CancellationToken cancellationToken)
        {
            // Check role of user performing action
            var userId = _claimInterface.User.UserId;
            
            var result = await _unitOfWork.AssignmentRepository.GetAssignmentDetail(request.AssignmentId);
            if (result == null)
            {
                return null;
            }
            var zone = _mapper.Map<AssignmentDetailResponseModel>(result);
            if (_claimInterface.User.Role != (int)RoleEnum.Admin && _claimInterface.User.Role != (int)RoleEnum.Moderator)
            {
                var member = await _unitOfWork.ZoneMembershipRepository.GetMembership(userId, result.ZoneId);
                zone.SubmissionsCount = result.Submissions.Count;
                if (member != null)
                {
                    zone.Submitted = result.Submissions.Any(s => s.MemberId == member.Id);
                }
            }
            // If the user is not Admin/Moderator, check submission status
            else
            {
                // For Admin/Moderator, just populate submission 
                zone.SubmissionsCount = result.Submissions.Count;
            }
            //learner
            foreach (var member in zone.Submissions)
            {
                var response = await _dapr.InvokeMethodAsync<UserResponseDapr>(
                    HttpMethod.Get,
                    "user-sidecar",
                    $"api/v1/dapr/user?username={Uri.EscapeDataString(member.UserId.ToString())}",
                    cancellationToken
                );

                var author = new Author()
                {
                    AuthorId = Guid.Parse(response.UserId),
                    AuthorName = response.Username,
                    AuthorImage = response.Avatar
                };
                member.Learner = author;
            }
            return zone;

        }
    }
}

