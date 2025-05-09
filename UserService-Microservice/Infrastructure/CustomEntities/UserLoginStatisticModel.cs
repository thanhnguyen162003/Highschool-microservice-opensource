namespace Infrastructure.CustomEntities
{
    public class SessionStatisticModel
    {
        public DateTime Date { get; set; }
        public Dictionary<string, int> Data { get; set; } = new();
    }
}
