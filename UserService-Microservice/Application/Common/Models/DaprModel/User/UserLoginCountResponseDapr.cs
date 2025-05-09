namespace Application.Common.Models.DaprModel.User
{
    public class UserLoginCountResponseDapr
    {
        public List<UserRetentionDapr> Retention { get; set; } = new List<UserRetentionDapr>();
    }
    public class UserRetentionDapr
    {
        public string UserId { get; set; }
        public string Date { get; set; }
        public string RoleId { get; set; }
    }
}
