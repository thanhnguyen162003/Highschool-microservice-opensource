
namespace Domain.Entities
{
    public sealed class OutboxMessage
    {
		public Guid EventId { get; set; }
		public required string EventPayload { get; set; }
		public DateTime OccurredOn { get; set; }
		public DateTime ProcessedOn { get; set; }
		public bool IsMessageDispatched { get; set; }
		public string? Error { get; init; }
	}
}
