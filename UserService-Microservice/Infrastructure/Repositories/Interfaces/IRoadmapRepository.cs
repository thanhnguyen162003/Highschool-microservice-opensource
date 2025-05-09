namespace Infrastructure.Repositories.Interfaces
{
	public interface IRoadmapRepository : IRepository<Roadmap>
	{
		Task<Roadmap?> GetByUserId(Guid userId);
	}
}
