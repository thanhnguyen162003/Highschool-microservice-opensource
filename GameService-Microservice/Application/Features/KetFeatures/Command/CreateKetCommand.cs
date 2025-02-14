using Application.Common.Helper;
using Application.Messages;
using AutoMapper;
using Domain.Entity;
using Domain.Models.Common;
using Domain.Models.KetModels;
using FluentValidation;
using Infrastructure;
using MediatR;

namespace Application.Features.KetFeatures.Command
{
    public class CreateKetCommand : IRequest<APIResponse>
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Thumbnail { get; set; }

        public string? Status { get; set; }

        public IEnumerable<KetContentRequestModel>? KetContents { get; set; }
    }

    public class CreateKetCommandValidate : AbstractValidator<CreateKetCommand>
    {
        public CreateKetCommandValidate()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(500).WithMessage("Name cannot be more than 500 characters");

            RuleFor(x => x.Thumbnail).NotEmpty().WithMessage("Thumbnail is required");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(v => v.IsKetStatusValid()).WithMessage("Status is Public or Private");
        }
    }

    public class CreateKetCommandHandler : IRequestHandler<CreateKetCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateKetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse> Handle(CreateKetCommand request, CancellationToken cancellationToken)
        {
            var ket = _mapper.Map<Ket>(request);
            ket.TotalPlay = 0;

            await _unitOfWork.KetRepository.Add(ket);

            if (request.KetContents != null)
            {
                var ketContents = request.KetContents.Select((x, index) =>
                {
                    if ((index + 1) != x.Order)
                    {
                        throw new ValidationException($"Question {x.Question} have Order not valid. Please check again before create!");
                    }

                    if (x.Answers.Count() <= x.CorrectAnswer)
                    {
                        throw new ValidationException($"Correct answer is not number of answer");
                    }

                    var ketContent = _mapper.Map<KetContent>(x, opts =>
                    {
                        opts.Items["KetId"] = ket.Id;
                    });

                    return ketContent;
                });

                await _unitOfWork.KetContentRepository.Add(ketContents);
                ket.TotalQuestion = ketContents.Count();
            }

            if (await _unitOfWork.SaveChangesAsync())
            {
                return new APIResponse
                {
                    Status = System.Net.HttpStatusCode.Created,
                    Message = MessageCommon.CreateSuccesfully,
                    Data = ket.Id
                };
            }

            return new APIResponse
            {
                Status = System.Net.HttpStatusCode.InternalServerError,
                Message = MessageCommon.CreateFailed
            };
        }
    }
}
