using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class HistoryPlayRepository : SQLRepository<HistoryPlay>, IHistoryPlayRepository
    {
        public HistoryPlayRepository(DbContext context) : base(context)
        {
        }

        public async Task Add(IEnumerable<HistoryPlay> players)
        {
            await _dbSet.AddRangeAsync(players);
        }

    }
}
