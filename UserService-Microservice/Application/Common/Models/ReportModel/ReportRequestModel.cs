namespace Application.Common.Models.ReportModel
{
	public class ReportRequestModel
	{
		public string ReportTitle { get; set; } = null!;

		public string ReportContent { get; set; } = null!;

		public List<IFormFile>? Images { get; set; }
	}
}
