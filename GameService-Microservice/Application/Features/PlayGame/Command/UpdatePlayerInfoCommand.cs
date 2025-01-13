using Application.Messages;
using Application.Services.RealTime;
using Domain.Constants;
using Domain.Models.Common;
using Infrastructure;
using MediatR;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class UpdatePlayerInfoCommand : IRequest<APIResponse>
    {
        public Guid UserId { get; set; }
        public string RoomId { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Avatar { get; set; }
    }

    public class UpdatePlayerInfoCommandHandler : IRequestHandler<UpdatePlayerInfoCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAblyService _ablyService;

        public UpdatePlayerInfoCommandHandler(IUnitOfWork unitOfWork, IAblyService ablyService)
        {
            _unitOfWork = unitOfWork;
            _ablyService = ablyService;
        }

        public async Task<APIResponse> Handle(UpdatePlayerInfoCommand request, CancellationToken cancellationToken)
        {
            var player = await _unitOfWork.LeaderboardRepository.UpdatePlayer(request.RoomId, request.UserId, request.Avatar, request.DisplayName);

            if (player != null)
            {
                var result = await _ablyService.SendMessage(request.RoomId, new SocketResponse()
                {
                    Type = AblyConstant.UpdatePlayerInfoEvent,
                    Data = player
                });

                if (result.Status != HttpStatusCode.OK)
                {
                    return new APIResponse()
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = MessageGame.UpdatePlayerInfoFail,
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
                    Message = MessageGame.UpdatePlayerInfoSuccess
                };
            }

            return new APIResponse
            {
                Status = HttpStatusCode.BadRequest,
                Data = MessageCommon.NotFound
            };

        }
    }

}
