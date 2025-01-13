using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IKetContentRepository : ISQLRepository<KetContent>
    {
        Task Add(IEnumerable<KetContent> ketContents);
        Task Delete(IEnumerable<KetContent> ketContents);
    }
}
