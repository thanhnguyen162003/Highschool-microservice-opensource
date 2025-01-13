using Application.Common.Helper;
using Application.Messages;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using FluentValidation;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.UserFeatures.Command
{
    public class CreateAvatarComand : IRequest<APIResponse>
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Rarity { get; set; }
        public string? Type { get; set; }
        public string? Background { get; set; }
    }

    public class CreateAvatarCommandValidate : AbstractValidator<CreateAvatarComand>
    {
        public CreateAvatarCommandValidate()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Image).NotEmpty().WithMessage("Image is required.");
            RuleFor(x => x.Rarity)
                .NotEmpty().WithMessage("Rarity is required.")
                .Must(x => x!.IsInEnum<AvatarRarity, string>()).WithMessage("Invalid Rarity.");


        }
    }

    public class CreateAvatarComandHandler : IRequestHandler<CreateAvatarComand, APIResponse>
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IMapper _mapper;

        public CreateAvatarComandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitofWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse> Handle(CreateAvatarComand request, CancellationToken cancellationToken)
        {
            var avatar = _mapper.Map<Avatar>(request);

            await _unitofWork.AvatarRepository.Add(avatar);

            if (await _unitofWork.SaveChangesAsync())
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageCommon.CreateSuccesfully,
                    Data = avatar.Id
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
