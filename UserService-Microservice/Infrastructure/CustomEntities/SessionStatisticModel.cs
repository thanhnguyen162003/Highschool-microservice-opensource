namespace Infrastructure.CustomEntities
{
    public class UserLoginStatisticModel
    {
        public string UserId { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Role { get; set; } 
    }
}
