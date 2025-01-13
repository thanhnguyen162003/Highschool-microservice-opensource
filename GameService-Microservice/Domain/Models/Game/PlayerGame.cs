namespace Domain.Models.PlayGameModels
{
    public class PlayerGame
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? RoomId { get; set; }
        public int Score { get; set; } = 0;
        public string Avatar { get; set; } = string.Empty;
        public int TimeAverage { get; set; } = 0;
        public int Rank { get; set; } = 0;
    }
}
