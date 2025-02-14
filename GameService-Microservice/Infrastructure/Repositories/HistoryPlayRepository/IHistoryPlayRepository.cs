using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IHistoryPlayRepository : ISQLRepository<HistoryPlay>
    {
        Task Add(IEnumerable<HistoryPlay> players);
    }
}
