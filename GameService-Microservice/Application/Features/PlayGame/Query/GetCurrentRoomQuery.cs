using Domain.Models.PlayGameModels;
using Infrastructure;
using MediatR;

namespace Application.Features.PlayGame.Querry
{
    public class GetCurrentRoomQuery : IRequest<LobbyInformationResponse>
    {
        public string RoomId { get; set; } = string.Empty;
    }

    public class GetCurrentRoomQueryHandler : IRequestHandler<GetCurrentRoomQuery, LobbyInformationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCurrentRoomQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LobbyInformationResponse> Handle(GetCurrentRoomQuery request, CancellationToken cancellationToken)
        {
            var room = await _unitOfWork.RoomGameRepository.GetById(request.RoomId);
            var players = await _unitOfWork.LeaderboardRepository.GetAll(request.RoomId);

            return new LobbyInformationResponse()
            {
                Room = room,
                Players = players
            };

        }
    }

}
