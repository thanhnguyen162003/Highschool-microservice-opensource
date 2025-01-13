using Domain.Enums;

namespace Domain.Models.Game
{
    public class RoomGame
    {
        public string Id { get; set; } = null!;
        public Guid KetId { get; set; }
        public Guid UserId { get; set; }
        public RoomStatus RoomStatus { get; set; }
        public int TotalQuestion { get; set; }
        public string? Thumbnail { get; set; }
        public string? Name { get; set; }
    }
}
