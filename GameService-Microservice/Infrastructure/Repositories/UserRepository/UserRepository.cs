using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : SQLRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

    }
}
