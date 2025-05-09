
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
	public class OutboxMessageRepository(UserDatabaseContext context) : IOutboxMessage
	{
		public async Task<bool> AddOutboxMessageAsync(OutboxMessage outboxMessage)
		{
			await context.OutboxMessages.AddAsync(outboxMessage);
			var result = await context.SaveChangesAsync();
			return result > 0;
		}
	}
}
