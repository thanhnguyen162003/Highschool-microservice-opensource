using Application.Common.Models.SubmissionContent;
using AutoMapper;
using Infrastructure.Repositories;
using MediatR;
using Application.Services.Authentication;
using System.Text.Json.Serialization;
using Domain.Entity;
using Domain.Enums;
using static Domain.Enums.ZoneEnums;
using Dapr.Client.Autogen.Grpc.v1;
using Domain.DaprModels;
using Application.Common.Models.OtherModel;
using Application.Common.Models.ZoneModel;
using Dapr.Client;

namespace Application.Features.AssignmentFeatures.Queries
{
    public record GetSubmissionQuery : IRequest<List<SubmissionResponseModel>>
    {
        [JsonIgnore]
        public Guid AssignmentId { get; set; }

    }

    public class GetSubmissionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService, DaprClient dapr) : IRequestHandler<GetSubmissionQuery, List<SubmissionResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly DaprClient _dapr = dapr;

        public async Task<List<SubmissionResponseModel>> Handle(GetSubmissionQuery request, CancellationToken cancellationToken)
        {
            // Check assignment exists
            var assignment = await _unitOfWork.AssignmentRepository.GetById(request.AssignmentId);
            List<Submission> result = new List<Submission>();
            if (assignment == null)
            {
                return null;
            }
            var zone = await _unitOfWork.ZoneRepository.GetById(assignment.ZoneId);
            var userId = _authenticationService.User.UserId;
            var membership = await _unitOfWork.ZoneMembershipRepository.GetMembership(userId, zone.Id);
            if (membership == null) 
            {
                if (assignment.CreatedBy == userId || _authenticationService.User.Role == (int)RoleEnum.Admin || _authenticationService.User.Role == (int)RoleEnum.Moderator)
                {
                    result = await _unitOfWork.SubmissionRepository.GetSubmissionForTeacher(request.AssignmentId);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (membership.Type == ZoneMembershipType.Teacher.ToString())
                {
                    result = await _unitOfWork.SubmissionRepository.GetSubmissionForTeacher(request.AssignmentId);
                }
                else result = await _unitOfWork.SubmissionRepository.GetSubmissionForStudent(request.AssignmentId, membership.Id);
            }
            var mapper = _mapper.Map<List<SubmissionResponseModel>>(result);
            //learner
            foreach (var member in mapper)
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

            return mapper;
        }
    }
}