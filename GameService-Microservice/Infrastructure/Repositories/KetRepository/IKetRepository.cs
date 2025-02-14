using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IKetRepository : ISQLRepository<Ket>
    {
        Task<Ket?> GetMyKet(Guid ketId, Guid userId);
        PagedList<Ket> GetAll(KetSortBy sortBy, bool isAscending, string? search, Guid? userId);
        Task<PagedList<Ket>> GetKetRecommend(IEnumerable<Ket> kets, int numberKet);
        Task<PagedList<Ket>> GetNewKets(int numberKet);
    }
}
