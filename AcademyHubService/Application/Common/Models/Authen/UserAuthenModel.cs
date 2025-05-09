namespace Application.Common.Models.Authen
{
    public class UserAuthenModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; }
        public Guid SessionId { get; set; }
        public bool IsAuthenticated { get; set; }

    }
}
