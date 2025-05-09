using Application.Common.Messages;
using Application.Messages;
using Application.Services.Authentication;
using Application.Services.KafkaService.Producer;
using Domain.Constants.Services;
using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Repositories;
using MediatR;
using SharedProjects.ConsumeModel;
using System.Net;
using static Domain.Enums.ZoneEnums;

namespace Application.Features.MemberFeatures.Commands
{
    public class ReplyInviteZoneCommand : IRequest<APIResponse>
    {
        public Guid ZoneId { get; set; }
        public RelyInvite RelyInvite { get; set; }
    }

    public class ReplyInviteZoneCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IProducerService producerService) : IRequestHandler<ReplyInviteZoneCommand, APIResponse>
    {   
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IProducerService _producerService = producerService;

        public async Task<APIResponse> Handle(ReplyInviteZoneCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;
            var email = _authenticationService.User.Email;
            var pendingZoneInvite = await _unitOfWork.PendingZoneInviteRepository.GetBy(x => x.ZoneId.Equals(request.ZoneId) && 
                                                        x.Email.Equals(email));

            if(pendingZoneInvite == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageZone.InviteIsNotFound
                };
            } else if(pendingZoneInvite.ExpiredAt < DateTime.UtcNow)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageZone.InviteIsExpired
                };
            }

            if (request.RelyInvite == RelyInvite.Accept)
            {
                await _unitOfWork.ZoneMembershipRepository.Add(new ZoneMembership()
                {
                    Email = email,
                    Type = request.RelyInvite.ToString(),
                    ZoneId = request.ZoneId,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                    InviteBy = pendingZoneInvite.InviteBy
                });
            }

            await _unitOfWork.PendingZoneInviteRepository.Delete(pendingZoneInvite.Id);
            
            if(await _unitOfWork.SaveChangesAsync())
            {
                if(request.RelyInvite == RelyInvite.Accept){
                    var zone = await _unitOfWork.ZoneRepository.GetBy(x => x.Id.Equals(request.ZoneId));
                    if (zone != null)
                    {
                        var dataModel = new NotificationUserModel()
                        {
                            UserId = userId.ToString(),
                            Content = $"Bạn đã được mời tham gia vào zone {zone.Name}",
                            Title = "Zone Invitation"
                        };
                        var dataModelOwner = new NotificationUserModel()
                        {
                            UserId = zone.CreatedBy.ToString(),
                            Content = $"Người dùng với mail {email} đã tham gia zone {zone.Name}",
                            Title = "Zone Invitation Notification"
                        };
                        var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.NotificationUserCreated, userId.ToString(), dataModel);
                        var resultOwner = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.NotificationUserCreated, zone.CreatedBy.ToString(), dataModelOwner);
                    }
                }
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = request.RelyInvite == RelyInvite.Accept ? MessageZone.JoinZoneSuccess : MessageZone.RejectInviteSuccess,
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
