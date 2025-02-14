using Application.Messages;
using Application.Services.RealTime;
using Domain.Constants;
using Domain.Models.Common;
using Infrastructure;
using FluentValidation;
using MediatR;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class NextQuestionCommand : IRequest<APIResponse>
    {
        public string RoomId { get; set; } = null!;
        public int Order { get; set; }
    }

    public class NextQuestionCommandValidator : AbstractValidator<NextQuestionCommand>
    {
        public NextQuestionCommandValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty().WithMessage("RoomId is required")
                .Length(6).WithMessage("RoomId must be 6 characters");


            RuleFor(x => x.Order)
                .NotEmpty().WithMessage("Order is required");
        }

    }

    public class NextQuestionCommandHandler : IRequestHandler<NextQuestionCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        private readonly IAblyService _ablyService;

        public NextQuestionCommandHandler(IUnitOfWork unitOfWork, ISender sender, IAblyService ablyService)
        {
            _unitOfWork = unitOfWork;
            _sender = sender;
            _ablyService = ablyService;
        }

        public async Task<APIResponse> Handle(NextQuestionCommand request, CancellationToken cancellationToken)
        {
            var question = await _unitOfWork.QuestionGameRepository.GetQuestion(request.RoomId, request.Order);

            if (question == null)
            {
                return await _sender.Send(new FinishGameCommand(), cancellationToken);
            }

            var players = await _unitOfWork.LeaderboardRepository.GetAll(request.RoomId);

            var result = await _ablyService.SendMessage(request.RoomId, new SocketResponse()
            {
                Type = AblyConstant.NewQuestionEvent,
                Data = question
            });

            if (result.Status != HttpStatusCode.OK)
            {
                return result;
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.OK,
                Message = MessageGame.StartGameSuccess,
                Data = players
            };
        }
    }

}
