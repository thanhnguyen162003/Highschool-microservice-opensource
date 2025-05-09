using Application.Common.Models.UserModel;

namespace Application.Common.Models.ReportDocumentModel
{
	public class ReportDocumentResponseModel
	{
		public Guid Id { get; set; }
		public string? ReportTitle { get; set; }
		public string? ReportContent { get; set; }
		public string? Status { get; set; }
		public Guid? DocumentId { get; set; }
		public string? ReportType { get; set; }
		public Guid UserId { get; set; }
		public DateTime CreatedAt { get; set; }
		public BaseUserResponse User { get; set; }
	}
}
