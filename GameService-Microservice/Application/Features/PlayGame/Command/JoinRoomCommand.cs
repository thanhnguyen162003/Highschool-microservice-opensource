using Application.Messages;
using Application.Services.Authentication;
using Application.Services.RealTime;
using Domain.Constants;
using Domain.Models.Common;
using Domain.Models.PlayGameModels;
using Infrastructure;
using FluentValidation;
using MediatR;

namespace Application.Features.PlayGame.Command
{
    public class JoinRoomCommand : IRequest<APIResponse>
    {
        public string RoomId { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Avatar { get; set; } = null!;
    }

    public class JoinRoomCommandValidator : AbstractValidator<JoinRoomCommand>
    {
        private readonly IUnitOfWork _unitOfWork;


        public JoinRoomCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Configuration();
        }

        private void Configuration()
        {
            RuleFor(p => p.RoomId)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MustAsync(ExistRoom).WithMessage("Room is not exist.")
                .NotNull();

            RuleFor(p => p.DisplayName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(p => p.Avatar)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();
        }

        private async Task<bool> ExistRoom(string roomId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.RoomGameRepository.IsExistRoom(roomId);
        }
    }

    public class JoinRoomCommandHandler : IRequestHandler<JoinRoomCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAblyService _ablyService;

        public JoinRoomCommandHandler(IUnitOfWork unitOfWork, IAblyService ablyService,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _ablyService = ablyService;
        }

        public async Task<APIResponse> Handle(JoinRoomCommand request, CancellationToken cancellationToken)
        {
            Guid userId;
            if (_authenticationService.IsAuthenticated())
            {
                userId = _authenticationService.GetUserId();

                var existPlayer = await _unitOfWork.LeaderboardRepository.GetById(request.RoomId, userId);

                if (existPlayer != null)
                {
                    return new APIResponse()
                    {
                        Status = System.Net.HttpStatusCode.BadRequest,
                        Message = MessageGame.JoinRoomSuccess,
                        Data = existPlayer
                    };
                }

            } else
            {
                userId = Guid.NewGuid();
            }

            var player = new PlayerGame()
            {
                Id = userId,
                DisplayName = request.DisplayName,
                Avatar = request.Avatar,
                RoomId = request.RoomId,
                Score = 0,
                TimeAverage = 0
            };

            await _unitOfWork.LeaderboardRepository.AddPlayer(request.RoomId, player);

            // Publish player joined event
            var result = await _ablyService.SendMessage(request.RoomId, new SocketResponse()
            {
                Type = AblyConstant.PlayerJoinedEvent,
                Data = player
            });

            if (result.Status != System.Net.HttpStatusCode.OK)
            {
                return new APIResponse()
                {
                    Status = System.Net.HttpStatusCode.InternalServerError,
                    Message = MessageGame.JoinRoomFail,
                    Data = new
                    {
                        Message = result.Message,
                        Data = result.Data
                    }
                };
            }

            //var players = await _unitOfWork.LeaderboardRepository.GetAll(request.RoomId);

            return new APIResponse()
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = MessageGame.JoinRoomSuccess,
                Data = player
            };

        }
    }
}
