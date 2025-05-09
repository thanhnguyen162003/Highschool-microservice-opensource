using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GroupRepository(DbContext context) : SqlRepository<Group>(context), IGroupRepository
    {
    }
}
