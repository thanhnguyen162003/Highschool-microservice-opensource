using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ZoneBanRepository(DbContext context) : SqlRepository<ZoneBan>(context), IZoneBanRepository
    {
    }
}
