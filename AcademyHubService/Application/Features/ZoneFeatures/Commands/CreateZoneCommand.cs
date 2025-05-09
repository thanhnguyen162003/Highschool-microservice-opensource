using Application.Common.Helper;
using Application.Messages;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using FluentValidation;
using Infrastructure.Repositories;
using MediatR;
using System.Net;
using static Domain.Enums.ZoneEnums;

namespace Application.Features.ZoneFeatures.Commands
{
    public class CreateZoneCommand : IRequest<APIResponse>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
    }

    public class CreateZoneCommandValidator : AbstractValidator<CreateZoneCommand>
    {
        public CreateZoneCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotNull().WithMessage("{PropertyName} is required.")
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.LogoUrl)
                .Must(Utils.IsValidUrl).WithMessage("{PropertyName} must be a valid URL.");

        }
    }

    public class CreateZoneCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService) : IRequestHandler<CreateZoneCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<APIResponse> Handle(CreateZoneCommand request, CancellationToken cancellationToken)
        {
            // Create zone
            var zone = _mapper.Map<Zone>(request);
            zone.Status = ZoneStatusEnum.Available;
            zone.CreatedBy = _authenticationService.User.UserId;
            await _unitOfWork.ZoneRepository.Add(zone);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageCommon.CreateSuccesfully,
                    Data = zone.Id
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.CreateFailed
            };
        }
    }

}
