namespace Domain.Entities;

public class Report
{
	public Guid Id { get; set; }

	public string ReportTitle { get; set; } = null!;

	public string ReportContent { get; set; } = null!;

	public string Status { get; set; } = null!;

	public Guid UserId { get; set; }

	public DateTime CreatedAt { get; set; }

	public virtual BaseUser User { get; set; } = null!;

	public virtual ICollection<ImageReport> ImageReports { get; set; } = new HashSet<ImageReport>();
}
