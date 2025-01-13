using Application.Messages;
using Application.Services.BackgroundService.ServiceTask.Common;
using Application.Services.RealTime;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Common;
using Domain.Models.PlayGameModels;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class StartGameCommand : IRequest<APIResponse>
    {
        public string RoomId { get; set; } = string.Empty;
    }

    public class StartGameCommandHandler : IRequestHandler<StartGameCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAblyService _ablyService;

        public StartGameCommandHandler(IUnitOfWork unitOfWork, IAblyService ablyService, ICommonTask commonTask)
        {
            _unitOfWork = unitOfWork;
            _ablyService = ablyService;
        }

        public async Task<APIResponse> Handle(StartGameCommand request, CancellationToken cancellationToken)
        {
            var room = await _unitOfWork.RoomGameRepository.GetById(request.RoomId);

            if (room == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageGame.RoomNotFound,
                    Data = request.RoomId
                };
            }

            var players = await _unitOfWork.LeaderboardRepository.GetAll(request.RoomId);

            var question = await _unitOfWork.QuestionGameRepository.GetQuestion(request.RoomId, 1);

            if (question == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageGame.QuestionNotFound,
                    Data = request.RoomId
                };
            }

            var result = await _ablyService.SendMessage(request.RoomId, new SocketResponse()
            {
                Type = AblyConstant.GameStartedEvent,
                Data = question
            });

            if (result.Status == HttpStatusCode.OK)
            {
                room.RoomStatus = RoomStatus.Playing;
                await _unitOfWork.RoomGameRepository.Update(room);

                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageGame.StartGameSuccess,
                    Data = new RoundGameModel()
                    {
                        Order = 1,
                        TotalQuestion = room.TotalQuestion,
                        Players = players
                    }
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageGame.StartGameFail,
                Data = result.Data
            };

        }
    }
}
