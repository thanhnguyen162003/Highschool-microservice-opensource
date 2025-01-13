namespace Domain.Models.PlayGameModels
{
    public class RoundGameModel
    {
        public int Order { get; set; }
        public int TotalQuestion { get; set; }
        public IEnumerable<PlayerGame> Players { get; set; } = new List<PlayerGame>();
    }
}
