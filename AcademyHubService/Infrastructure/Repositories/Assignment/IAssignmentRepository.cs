using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IAssignmentRepository : ISqlRepository<Assignment>
    {
        Task<PagedList<Assignment>> GetAssignment(int page, int pageSize, string? search, bool isAscending);
        Task<Assignment> GetAssignmentDetail(Guid assignmentId);
        Task<List<Assignment>> GetAssignmentByZoneId(Guid zoneId);
        Task<Dictionary<Guid, int>> GetAssignmentCountByZoneId(Guid zoneId);
    }
}
