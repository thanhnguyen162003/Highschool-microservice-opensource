using Application.Common.Models.UserModel;

namespace Application.Common.Models.ReportModel
{
	public class ReportResponseModel
	{
		public Guid Id { get; set; }

		public string ReportTitle { get; set; } = null!;

		public string ReportContent { get; set; } = null!;

		public string Status { get; set; } = null!;

		public Guid UserId { get; set; }

		public string FullName { get; set; } = null!;

		public string Email { get; set; } = null!;

		public DateTime CreatedAt { get; set; }

		public IEnumerable<string> ImageReports { get; set; } = new List<string>();

		public BaseUserResponse? User { get; set; }
    }
}
