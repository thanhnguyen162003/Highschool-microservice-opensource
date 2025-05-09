using Application.Common.Messages;
using Application.Messages;
using Application.Services.Authentication;
using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using System.Net;

namespace Application.Features.MemberFeatures.Commands
{
    public class DeleteMemberCommand : IRequest<APIResponse>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public Guid ZoneId { get; set; }
        public bool? IsBanned { get; set; }
        public string? Reason { get; set; }
    }

    public class DeleteMemberCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService) : IRequestHandler<DeleteMemberCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
        {
            // Remove the member from the zone if they are a member
            var zoneMembership = await _unitOfWork.ZoneMembershipRepository.GetMembership(request.UserId, request.ZoneId);

            if(zoneMembership != null)
            {
                await _unitOfWork.ZoneMembershipRepository.Delete(zoneMembership.Id);
            }

            // Add the member to the banned list
            if(request.IsBanned == true)
            {
                var userId = _authenticationService.User.UserId;
                await _unitOfWork.ZoneBanRepository.Add(new ZoneBan()
                {
                    Email = request.Email,
                    UserId = request.UserId,
                    ZoneId = request.ZoneId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    Reason = request.Reason,
                });
            }

            if(await _unitOfWork.SaveChangesAsync())
            {
                // Send mail to the user

                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageZone.MemberBanned,
                    Data = request.ZoneId
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
