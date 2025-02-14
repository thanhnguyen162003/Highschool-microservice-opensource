using Application.Common.Helper;
using Application.Messages;
using Application.Services.RealTime;
using Domain.Constants;
using Domain.Models.Common;
using Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;

namespace Application.Features.PlayGame.Command
{
    public class SelectAnswerCommand : IRequest<APIResponse>
    {
        public int OrderQuestion { get; set; }
        public string RoomId { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public Guid UserId { get; set; }
        public int TimeAverage { get; set; }
        public int TimeQuestion { get; set; }
    }

    public class SelectAnswerCommandHandler : IRequestHandler<SelectAnswerCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SettingGameDefault _setting;
        private readonly IAblyService _ablyService;

        public SelectAnswerCommandHandler(IUnitOfWork unitOfWork, IOptions<SettingGameDefault> options, IAblyService ablyService)
        {
            _unitOfWork = unitOfWork;
            _setting = options.Value;
            _ablyService = ablyService;
        }

        public async Task<APIResponse> Handle(SelectAnswerCommand request, CancellationToken cancellationToken)
        {
            var score = Utils.CalculateScore(request.TimeAverage, _setting.MinTime, request.TimeQuestion, _setting.MinScore, _setting.MaxScore);
            var player = await _unitOfWork.LeaderboardRepository.UpdateProgress(request.RoomId, request.UserId, (int)score, request.TimeAverage);

            if (player == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = MessageGame.NetworkError
                };
            }

            // Publish player joined event
            var result = await _ablyService.SendMessage(request.RoomId, new SocketResponse()
            {
                Type = AblyConstant.PlayerSelectAnswerEvent,
                Data = player
            });

            if (result.Status != System.Net.HttpStatusCode.OK)
            {
                return new APIResponse()
                {
                    Status = System.Net.HttpStatusCode.InternalServerError,
                    Message = MessageGame.SelectAnswerFail,
                    Data = new
                    {
                        Message = result.Message,
                        Data = result.Data
                    }
                };
            }

            return new APIResponse()
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = MessageGame.SelectAnswerSuccess,
                Data = player
            };
        }
    }

}
