using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Domain.Enums.ZoneEnums;

namespace Infrastructure.Repositories
{
    public class ZoneMembershipRepository(DbContext context) : SqlRepository<ZoneMembership>(context), IZoneMembershipRepository
    {
        public async Task<int> GetZoneMemberCount()
        {
            var query =  await _dbSet
                .Where(z => z.DeletedAt == null && z.UserId != null)
                .GroupBy(x => x.UserId).CountAsync();
            return query;

        }
        public async Task<bool> IsTeacherInZone(Guid userId, Guid zoneId)
        {
            return await _dbSet.AnyAsync(x => x.UserId.Equals(userId) && x.ZoneId.Equals(zoneId) && x.Type.Equals(ZoneMembershipType.Teacher.ToString()) && x.DeletedAt == null);
        }

        public async Task<ZoneMembership?> GetMembership(Guid userId, Guid zoneId, bool includeDeleted = false)
        {
            return await _dbSet.FirstOrDefaultAsync(x =>
                x.UserId.Equals(userId) &&
                x.ZoneId.Equals(zoneId) &&
                (includeDeleted || x.DeletedAt == null));
        }
        public async Task<Dictionary<string,int>> GetMembershipCount( Guid zoneId)
        {
            var result = await _dbSet
                .Where(x => x.ZoneId.Equals(zoneId) && x.DeletedAt == null)
                .GroupBy(x => x.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);
            return result;
        }
        public async Task<IEnumerable<Guid?>> GetZoneMembershipByStudentId(Guid studentId)
        {
            return await _dbSet
                .Where(x => x.UserId.Equals(studentId) && x.DeletedAt == null)
                .Select(x => x.ZoneId)
                .ToListAsync();
        }
        public async Task<bool> IsMembership(string email, Guid zoneId)
        {
            return await _dbSet.AnyAsync(x => x.ZoneId.Equals(zoneId) && x.Email!.Equals(email) && (x.DeletedAt == null));
        }

        public async Task<bool> IsMembershipByUserId(Guid userId, Guid zoneId)
        {
            return await _dbSet.AnyAsync(x => x.ZoneId.Equals(zoneId) && x.UserId!.Equals(userId) && (x.DeletedAt == null));
        }
        public async Task<IEnumerable<string>> CheckMemberInZone(IEnumerable<string> emails, Guid zoneId)
        {
            return await _dbSet
                .Where(x => x.ZoneId.Equals(zoneId) && emails.Contains(x.Email!) && x.DeletedAt == null)
                .Select(x => x.Email!)
                .ToListAsync();
        }

    }
}
