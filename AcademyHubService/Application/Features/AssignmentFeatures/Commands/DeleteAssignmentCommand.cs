using Application.Common.Messages;
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
    public class DeleteAssignmentCommand : IRequest<APIResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

    }
   
    public class DeleteAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService) : IRequestHandler<DeleteAssignmentCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
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

            var testContents = await _unitOfWork.TestContentRepository.GetAll(k => k.Assignmentid.Equals(request.Id));

            await _unitOfWork.TestContentRepository.Delete(testContents);
            await _unitOfWork.AssignmentRepository.Delete(assignment.Id);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.DeleteSuccessfully,
                    Data = assignment.ZoneId
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.DeleteFailed,
            };

        }
    }
}
