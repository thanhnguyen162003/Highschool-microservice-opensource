using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
	public class RoadmapRepository(UserDatabaseContext context) : BaseRepository<Roadmap>(context), IRoadmapRepository
	{
		private readonly UserDatabaseContext _context = context;

        public async Task<Roadmap?> GetByUserId(Guid userId)
		{
			return await _context.Roadmaps.FirstOrDefaultAsync(r => r.UserId.Equals(userId));
		}
	}
}
