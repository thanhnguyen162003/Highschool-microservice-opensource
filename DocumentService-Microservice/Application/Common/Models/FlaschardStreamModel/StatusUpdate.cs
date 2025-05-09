using Domain.Enums;

namespace Application.Common.Models.FlaschardStreamModel
{
	public class StatusUpdate
	{
		public StreamingStatus Status { get; set; }
		public int TotalItems { get; set; }
		public int ProcessedItems { get; set; }
		public string Error { get; set; }
	}
}
