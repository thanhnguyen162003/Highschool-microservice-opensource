namespace Domain.Entities
{
	public class ImageReport
	{
		public Guid? ReportId { get; set; }
		public string? ImageUrl { get; set; }

		public virtual Report? Report { get; set; }
	}
}
