using Application.Common.Helper;
using Application.Common.Messages;
using Application.Common.Models.ProduceModels;
using Application.Common.Models.ZoneMembershipModel;
using Application.Messages;
using Application.Services.Authentication;
using Application.Services.KafkaService.Producer;
using Domain.Constants;
using Domain.Constants.Services;
using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using FluentValidation;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using static Domain.Enums.ZoneEnums;

namespace Application.Features.MemberFeatures.Commands
{
    public class InviteMemberCommand : IRequest<APIResponse>
    {
        public IEnumerable<InviteMemberRequestModel> Members { get; set; } = new List<InviteMemberRequestModel>();
        public Guid? ZoneId { get; set; }
    }

    public class MembersValidator : AbstractValidator<InviteMemberRequestModel>
    {
        public MembersValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .NotNull().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required")
                .NotNull().WithMessage("Type is required")
                .Must(x => x!.IsInEnum<ZoneMembershipType, string>());
        }
    }

    public class InviteMemberCommandValidator : AbstractValidator<InviteMemberCommand>
    {
        public InviteMemberCommandValidator()
        {
            

            RuleFor(x => x.Members)
                .NotEmpty().WithMessage("Members is required")
                .ForEach(x => x.SetValidator(new MembersValidator()));

            RuleFor(x => x.ZoneId)
                .NotEmpty().WithMessage("ZoneId is required")
                .NotNull().WithMessage("ZoneId is required");
        }
    }

    public class InviteMemberCommandHandler(IUnitOfWork unitOfWork, IProducerService producerService, IAuthenticationService authenticationService) : IRequestHandler<InviteMemberCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IProducerService _producerService = producerService;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
        {
            // Check if zone exists
            var zone = await _unitOfWork.ZoneRepository.GetById(request.ZoneId!);

            if (zone == null)
            {
                return new APIResponse
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound
                };
            }

            // Check if user is already a member
            var emails = request.Members.Select(m => m.Email).ToHashSet();
            var existingMembers = await _unitOfWork.ZoneMembershipRepository.CheckMemberInZone(emails!, (Guid)request.ZoneId!);
            if(!existingMembers.IsNullOrEmpty())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = MessageZone.MemberExists,
                    Data = existingMembers
                };
            }

            // Check if user is already invited
            var existingInvites = (await _unitOfWork.PendingZoneInviteRepository.GetAll(e =>
                e.ZoneId.Equals(request.ZoneId) && emails.Contains(e.Email)
            )).ToDictionary(x => x.Email);

            var pendingInvites = new List<PendingZoneInvite>();
            var newInvites = new List<PendingZoneInvite>();
            var cannotInvites = new List<InviteMemberRequestModel>();
            var userId = _authenticationService.User.UserId;

            foreach (var member in request.Members)
            {
                if(existingInvites.TryGetValue(member.Email!, out var invite))
                {
                    if(DateTime.UtcNow < invite.ExpiredAt)
                    {
                        cannotInvites.Add(member);
                    } else
                    {
                        invite.UpdatedAt = DateTime.UtcNow;
                        invite.Type = member.Type!;
                        invite.InviteBy = userId;
                        invite.ExpiredAt = DateTime.UtcNow.AddHours(24);
                        pendingInvites.Add(invite);
                    }
                } else
                {
                    newInvites.Add(new PendingZoneInvite()
                    {
                        Email = member.Email!,
                        Type = member.Type!,
                        CreatedAt = DateTime.UtcNow,
                        ZoneId = request.ZoneId!,
                        InviteBy = userId,
                        ExpiredAt = DateTime.UtcNow.AddHours(24)
                    });
                }
            }

            if(cannotInvites.Any()) return new APIResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageZone.CannotInviteAfter24h,
                Data = cannotInvites
            };

            if(pendingInvites.Any()) await _unitOfWork.PendingZoneInviteRepository.UpdateRange(pendingInvites);
            if (newInvites.Any()) await _unitOfWork.PendingZoneInviteRepository.AddRange(newInvites);

            if (await _unitOfWork.SaveChangesAsync())
            {
                // Send mail invite
                var result = await _producerService.ProduceObjectWithKeyAsync(TopicKafkaConstaints.MailZoneCreated, userId.ToString(), new MailModel()
                {
                    MailType = MailSendType.InviteMember,
                    MailInviteMemberModels = request.Members.Select(x => new MailInviteMemberModel()
                    {
                        CreatedBy = (Guid)zone.CreatedBy!,
                        Email = x.Email!,
                        LogoUrl = zone.LogoUrl!,
                        ZoneName = zone.Name!,
                        CreatedAt = DateTime.UtcNow,
                        Type = x.Type!,
                        Description = zone.Description!,
                        BannerUrl = zone.BannerUrl!,
                        AcceptLink = $"{UrlConstant.ClientUrl}/zone/{zone.Id}/reply/accept",
                        RejectLink = $"{UrlConstant.ClientUrl}/zone/{zone.Id}/reply/reject"
                    })
                });

                if (result)
                {
                    return new APIResponse()
                    {
                        Status = HttpStatusCode.OK,
                        Message = MessageZone.InviteMemberSuccess,
                        Data = zone.Id
                    };

                }
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.ServerError
            };

        }
    }

}
