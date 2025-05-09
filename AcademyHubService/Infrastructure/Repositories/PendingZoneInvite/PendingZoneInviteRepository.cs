using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PendingZoneInviteRepository(DbContext context) : SqlRepository<PendingZoneInvite>(context), IPendingZoneInviteRepository
    {
    }
}
