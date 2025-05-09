namespace Application.Common.Models.StatisticModel
{
	public class UserActivityResponseModel
    {
        public DateTime Date { get; set; }
        public int Students { get; set; }
        public int Teachers { get; set; }
        public int Moderators { get; set; }
    }
}
