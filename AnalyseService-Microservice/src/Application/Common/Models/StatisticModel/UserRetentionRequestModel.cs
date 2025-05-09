namespace Application.Common.Models.StatisticModel
{
	public class UserRetentionRequestModel
    {
        public Guid UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public int RoleId { get; set; }
    }
}
