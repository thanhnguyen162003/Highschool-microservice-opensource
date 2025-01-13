using Application.Messages;
using Application.Services.RealTime;
using Domain.Constants;
using Domain.Models.Common;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class KickPlayerCommand : IRequest<APIResponse>
    {
        public Guid UserId { get; set; }
        public string RoomId { get; set; } = null!;
    }

    public class KickPlayerCommandHandler : IRequestHandler<KickPlayerCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAblyService _ablyService;

        public KickPlayerCommandHandler(IUnitOfWork unitOfWork, IAblyService ablyService)
        {
            _unitOfWork = unitOfWork;
            _ablyService = ablyService;
        }

        public async Task<APIResponse> Handle(KickPlayerCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.LeaderboardRepository.Delete(request.RoomId, request.UserId);

            // Send message to the user
            var result = await _ablyService.SendMessage(request.RoomId, new SocketResponse()
            {
                Type = AblyConstant.PlayerKickedEvent,
                Data = request.UserId
            });

            if (result.Status != HttpStatusCode.OK)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = MessageGame.JoinRoomFail,
                    Data = new
                    {
                        result.Message,
                        result.Data
                    }
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.OK,
                Message = MessageGame.KickPlayerSuccess
            };

        }
    }

}
