using Application.Common.Models.TestContent;
using Application.Messages;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using FluentValidation;
using Infrastructure.Repositories;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.AssignmentFeatures.Commands
{
    public class ChangeAssignmentStatusCommand : IRequest<APIResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public bool Published { get; set; }
    }
    
    public class ChangeAssignmentStatusCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService) : IRequestHandler<ChangeAssignmentStatusCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(ChangeAssignmentStatusCommand request, CancellationToken cancellationToken)
        {
            // Check assignment exists
            var assignment = await _unitOfWork.AssignmentRepository.GetById(request.Id);

            if (assignment == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.Id
                };
            }
            if (assignment.Published == request.Published)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Assignment đã và đang "+ request.Published + " nên không cần update",
                    Data = request.Id
                };
            }
            var zone = await _unitOfWork.ZoneRepository.GetById(assignment.ZoneId);
            // Check role of user performing action
            var userId = _authenticationService.User.UserId;
            if (!await _unitOfWork.ZoneMembershipRepository.IsTeacherInZone(userId, assignment.ZoneId) && !zone.CreatedBy.Equals(userId))
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = MessageCommon.Forbidden
                };
            }

            assignment.Published = request.Published;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.AssignmentRepository.Update(assignment);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully,
                    Data = assignment.ZoneId
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.UpdateFailed,
            };

        }
    }
}
