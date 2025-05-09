
namespace Infrastructure.Repositories.Interfaces
{
    public interface IOutboxMessage
    {
        Task<bool> AddOutboxMessageAsync(OutboxMessage outboxMessage);
    }
}
