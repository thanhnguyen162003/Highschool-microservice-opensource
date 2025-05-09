using Domain.Enumerations;

namespace Application.Common.Models.ReportDocumentModel
{
	public class CreateReportDocumentRequestModel
	{
		public string ReportTitle { get; set; } = null!;
		public string ReportContent { get; set; } = null!;
		public ReportType ReportType { get; set; }
		public Guid DocumentId { get; set; }
	}
}
