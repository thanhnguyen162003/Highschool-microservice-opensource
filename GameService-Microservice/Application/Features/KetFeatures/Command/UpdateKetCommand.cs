using Application.Common.Helper;
using Application.Messages;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using Domain.Models.KetModels;
using FluentValidation;
using Infrastructure;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.KetFeatures.Command
{
    public class UpdateKetCommand : IRequest<APIResponse>
    {
        [JsonIgnore]
        public Guid KetId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Thumbnail { get; set; }

        public string? Status { get; set; }

        public IEnumerable<KetContentRequestModel>? KetContents { get; set; }
    }

    public class ValidateUpdateKetCommand : AbstractValidator<CreateKetCommand>
    {
        public ValidateUpdateKetCommand()
        {
            RuleFor(x => x.Name)
                .MaximumLength(500).WithMessage("Name cannot be more than 500 characters");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(v => v.IsKetStatusValid()).WithMessage("Status is Public or Private");
        }
    }

    public class UpdateKetCommandHandler : IRequestHandler<UpdateKetCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _authenticationService;

        public UpdateKetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authenticationService = authenticationService;
        }

        public async Task<APIResponse> Handle(UpdateKetCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.GetUserId();
            var ket = await _unitOfWork.KetRepository.GetMyKet(request.KetId, userId);

            if (ket == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.KetId
                };
            }

            ket = _mapper.Map(request, ket);

            if (request.KetContents != null)
            {
                var ketContentsExist = await _unitOfWork.KetContentRepository.GetAll(kc => kc.KetId.Equals(ket.Id));

                if (ketContentsExist != null)
                {
                    await _unitOfWork.KetContentRepository.Delete(ketContentsExist);
                }

                var ketContents = request.KetContents.Select((x, index) =>
                {
                    if ((index + 1) != x.Order)
                    {
                        throw new ValidationException($"Question {x.Question} have Order not valid. Please check again before create!");
                    }

                    var ketContent = _mapper.Map<KetContent>(x, opts =>
                    {
                        opts.Items["KetId"] = ket.Id;
                    });

                    return ketContent;
                });

                await _unitOfWork.KetContentRepository.Add(ketContents);
            }

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully,
                    Data = ket.Id
                };
            }

            return new APIResponse
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.UpdateFailed
            };
        }
    }
}
