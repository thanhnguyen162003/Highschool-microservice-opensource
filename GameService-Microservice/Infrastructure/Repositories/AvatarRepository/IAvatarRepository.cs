using Domain.Entity;
using Domain.Enums;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IAvatarRepository : ISQLRepository<Avatar>
    {
        Task<IEnumerable<Avatar>> GetAll(AvatarSortBy sortBy, bool IsAscending);
        Task UpdateBackground(string image, string type);
    }
}
