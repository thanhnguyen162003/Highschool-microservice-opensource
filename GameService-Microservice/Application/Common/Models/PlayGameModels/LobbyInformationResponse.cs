using Domain.Models.Game;

namespace Domain.Models.PlayGameModels
{
    public class LobbyInformationResponse
    {
        public RoomGame? Room { get; set; }
        public IEnumerable<PlayerGame>? Players { get; set; }
    }
}
