using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IZoneRepository : ISqlRepository<Zone>
    {
        Task<PagedList<Zone>> GetAllZone(int page, int pageSize, string? search, bool isAscending);
        Task<PagedList<Zone>> GetAllZoneForTeacher(int page, int pageSize, string? search, bool isAscending, Guid userId, IEnumerable<Guid> zoneIds);
        Task<PagedList<Zone>> GetAllZoneForStudent(int page, int pageSize, string? search, bool isAscending, Guid userId);
        Task<Zone> GetZoneDetail(Guid zoneId);
        Task<Dictionary<string, int>> GetZoneSubmissionScore(Guid zoneId);
        Task<int> GetZoneCreationCount();
    }
}