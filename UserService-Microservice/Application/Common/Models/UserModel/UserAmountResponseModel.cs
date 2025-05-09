namespace Application.Common.Models.UserModel
{
	public class UserAmountResponseModel
    {
        public int TotalUser { get; set; }
        public int ThisMonthUser { get; set; }
        public double IncreaseUserPercent { get; set; }
        public int TotalStudent { get; set; }
        public int TotalTeacher { get; set; }
        public int TotalActive { get; set; }
        public int TotalBlocked { get; set; }
        public int TotalDeleted { get; set; }
    }
}
