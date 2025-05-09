using Application.Messages;
using Application.Services.Authentication;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using System.Net;

namespace Application.Features.ZoneFeatures.Commands
{
    public class LeaveZoneCommand : IRequest<APIResponse>
    {
        public Guid ZoneId { get; set; }
    }

    public class LeaveZoneCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService) : IRequestHandler<LeaveZoneCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(LeaveZoneCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;
            var member = await _unitOfWork.ZoneMembershipRepository.GetMembership(userId, request.ZoneId);

            if (member == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Bạn không phải thành viên của zone này"
                };
            }
            member.DeletedAt = DateTime.UtcNow;
            await _unitOfWork.ZoneMembershipRepository.Update(member);

            if(await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = "Rời thành công"
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.ServerError
            };
        }
    }

}
