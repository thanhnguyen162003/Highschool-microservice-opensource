namespace Application.Common.Models.DaprModel.User
{
    public class UserCountResponseDapr
    {
        public List<UserActivityDapr> Activities { get; set; } = new List<UserActivityDapr>();
    }
    public class UserActivityDapr
    {
        public string Date { get; set; }
        public int Students { get; set; }
        public int Teachers { get; set; }
        public int Moderators { get; set; }
    }
}
