namespace Application.Common.Models.DaprModel.User
{ 
    public class UserResponseMediaDapr
    {
        public List<string> UserId { get; set; } = new List<string>();
        public List<string> Username { get; set; } = new List<string>();
        public List<string> Avatar { get; set; } = new List<string>();
    }
}
