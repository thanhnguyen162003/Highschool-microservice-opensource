using Domain.Entity;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IZoneMembershipRepository : ISqlRepository<ZoneMembership>
    {
        Task<bool> IsTeacherInZone(Guid userId, Guid zoneId);
        Task<ZoneMembership?> GetMembership(Guid userId, Guid zoneId, bool includeDeleted = false);
        Task<bool> IsMembershipByUserId(Guid userId, Guid zoneId);
        Task<bool> IsMembership(string email, Guid zoneId);
        Task<IEnumerable<string>> CheckMemberInZone(IEnumerable<string> emails, Guid zoneId);
        Task<IEnumerable<Guid?>> GetZoneMembershipByStudentId(Guid studentId);
        Task<Dictionary<string, int>> GetMembershipCount(Guid zoneId);
        Task<int> GetZoneMemberCount();
    }
}