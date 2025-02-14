namespace Domain.Entity
{
    public class Avatar
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int? Rarity { get; set; }
        public string? Type { get; set; }
        public string? Background { get; set; }
    }
}
