using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
	public class ContainerRepository(DocumentDbContext context) : BaseRepository<Container>(context), IContainerRepository
	{
		public async Task<bool> CreateContainer(Container container, CancellationToken cancellationToken)
		{
			await _entities.AddAsync(container, cancellationToken);
			var result = await context.SaveChangesAsync(cancellationToken);
			return result > 0;
		}

		public Task<bool> DeleteContainer(Guid containerId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<Container?> GetContainerByUserId(Guid userId, Guid flashcardId, CancellationToken cancellationToken)
		{
			var container = await _entities.FirstOrDefaultAsync(x => x.UserId == userId && x.FlashcardId == flashcardId, cancellationToken);
			return container;
		}

		public async Task<bool> UpdateContainer(Container container, CancellationToken cancellationToken)
		{
			_entities.Update(container);
			var result = await context.SaveChangesAsync(cancellationToken);
			return result > 0;
		}

	}
}
