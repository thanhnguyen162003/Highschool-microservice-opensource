namespace Application.Common.Models.DaprModel.User
{
    public class UserCountRequestDapr
    {
        public string Type { get; set; }
        public int Amount { get; set; }
        public bool IsCount { get; set; }
    }
}
