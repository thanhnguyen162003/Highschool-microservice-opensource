using Application.Common.Models.AssignmentModel;
using AutoMapper;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using Application.Services.Authentication;
using System.Net;
using Domain.Enums;
using Domain.Entity;

namespace Application.Features.AssignmentFeatures.Queries
{
    public record GetAssignmentQueryByZoneId : IRequest<List<AssignmentResponseModel>>
    {
        public Guid ZoneId { get; set; }
    }

    public class GetAssignmentQueryByZoneIdHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService) : IRequestHandler<GetAssignmentQueryByZoneId, List<AssignmentResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<List<AssignmentResponseModel>> Handle(GetAssignmentQueryByZoneId request, CancellationToken cancellationToken)
        {
            // Check role of user performing action
            var userId = _authenticationService.User.UserId;
            var result = await _unitOfWork.AssignmentRepository.GetAssignmentByZoneId(request.ZoneId);

            if (!result.Any())
            {
                return new List<AssignmentResponseModel>();
            }

            var mapper = _mapper.Map<List<AssignmentResponseModel>>(result);

            // If the user is not Admin/Moderator, check submission status
            if (_authenticationService.User.Role != (int)RoleEnum.Admin && _authenticationService.User.Role != (int)RoleEnum.Moderator)
            {
                var member = await _unitOfWork.ZoneMembershipRepository.GetMembership(userId, request.ZoneId);
                
                foreach (var assignment in mapper)
                {
                    var matchedAssignment = result.FirstOrDefault(x => x.Id == assignment.Id);
                    if (matchedAssignment != null)
                    {
                        assignment.SubmissionsCount = matchedAssignment.Submissions.Count;
                        if (member != null)
                        {
                            assignment.Submitted = matchedAssignment.Submissions.Any(s => s.MemberId == member.Id);
                        }
                    }
                }
                
            }
            else
            {
                // For Admin/Moderator, just populate submission count
                foreach (var assignment in mapper)
                {
                    var matchedAssignment = result.FirstOrDefault(x => x.Id == assignment.Id);
                    if (matchedAssignment != null)
                    {
                        assignment.SubmissionsCount = matchedAssignment.Submissions.Count;
                    }
                }
            }

            return mapper;

        }
    }
}