using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class KetContentRepository : SQLRepository<KetContent>, IKetContentRepository
    {
        public KetContentRepository(DbContext context) : base(context)
        {
        }

        public async Task Add(IEnumerable<KetContent> ketContents)
        {
            await _dbSet.AddRangeAsync(ketContents);
        }

        public async Task Delete(IEnumerable<KetContent> ketContents)
        {
            _dbSet.RemoveRange(ketContents);

            await Task.CompletedTask;
        }
    }
}
