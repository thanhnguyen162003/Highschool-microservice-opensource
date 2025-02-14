using Domain.Entity;
using Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
