using Domain.Enumerations;

namespace Domain.Entities
{
    public class ReportDocument
    {
		public Guid Id { get; set; }

		public string ReportTitle { get; set; } = null!;

		public string ReportContent { get; set; } = null!;

		public string Status { get; set; } = null!;

		public ReportType ReportType { get; set; }

		public required Guid DocumentId { get; set; }

		public Guid UserId { get; set; }

		public DateTime CreatedAt { get; set; }

		public virtual BaseUser User { get; set; } = null!;
	}
}
