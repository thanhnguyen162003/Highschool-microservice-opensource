using Application.Messages;
using Application.Services.Authentication;
using Application.Services.BackgroundService.ServiceTask.Common;
using Application.Services.RealTime;
using AutoMapper;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Common;
using Domain.Models.PlayGameModels;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class FinishGameCommand : IRequest<APIResponse>
    {
        public string RoomId { get; set; } = null!;
        public IEnumerable<PlayerGame> Players { get; set; } = null!;
    }

    public class FinishGameCommandHandler : IRequestHandler<FinishGameCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICommonTask _commonTask;
        private readonly IAblyService _ablyService;

        public FinishGameCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService,
            ICommonTask commonTask, IAblyService ablyService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _commonTask = commonTask;
            _ablyService = ablyService;
        }

        public async Task<APIResponse> Handle(FinishGameCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.GetUserId();
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

            room.RoomStatus = RoomStatus.Finished;

            var ket = await _unitOfWork.KetRepository.GetMyKet(room.KetId, userId);

            ket!.TotalPlay += 1;

            await _unitOfWork.BeginTransaction();

            await _unitOfWork.RoomGameRepository.Update(room);
            await _unitOfWork.KetRepository.Update(ket);

            if (await _unitOfWork.CommitTransaction())
            {
                // Save to redis before
                await _unitOfWork.LeaderboardRepository.Update(request.Players, request.RoomId);

                // update history for players
                _commonTask.AddSaveHistoryPlayer(request.RoomId);

                // finish game
                var response = await _ablyService.SendMessage(request.RoomId, new SocketResponse()
                {
                    Type = AblyConstant.GameFinishedEvent,
                    Data = MessageGame.FinishGame
                });

                if (response.Status != HttpStatusCode.OK)
                {
                    return response;
                }

                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageGame.FinishGame
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageGame.NetworkError
            };

        }
    }

}
