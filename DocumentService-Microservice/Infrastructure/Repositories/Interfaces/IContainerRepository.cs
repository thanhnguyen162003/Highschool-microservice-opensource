
namespace Infrastructure.Repositories.Interfaces
{
	public interface IContainerRepository : IRepository<Container>
	{
		Task<bool> CreateContainer(Container container, CancellationToken cancellationToken);
		Task<Container?> GetContainerByUserId(Guid userId, Guid flashcardId, CancellationToken cancellationToken);
		Task<bool> UpdateContainer(Container container, CancellationToken cancellationToken);
		Task<bool> DeleteContainer(Guid containerId, CancellationToken cancellationToken);
	}
}
