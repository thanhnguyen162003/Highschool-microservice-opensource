using Application.Common.Models.FlashcardModel;
using Domain.Enums;
using System.Reactive.Subjects;

namespace Application.Common.Models.FlaschardStreamModel
{
	public class FlashcardStreamingState
	{
		public Guid FlashcardId { get; set; }
		public Guid UserId { get; set; }
		public AIFlashcardRequestModel RequestModel { get; set; }
		public StreamingStatus Status { get; set; } = StreamingStatus.Initialized;
		public int TotalItems { get; set; }
		public int ProcessedItems { get; set; }
		public Subject<object> Updates { get; } = new Subject<object>();
	}
}
