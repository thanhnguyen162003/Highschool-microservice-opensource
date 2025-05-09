namespace Application.Common.Models.ReportModel
{
	public class ReportAmountResponseModel
    {
        public int TotalReport { get; set; }
        public int ThisMonthReport { get; set; }
        public double IncreaseReportPercent { get; set; }
        public int TotalReportType { get; set; }
    }
}
