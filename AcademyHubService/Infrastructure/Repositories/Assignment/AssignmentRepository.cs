using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Extensions;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories
{
    public class AssignmentRepository(DbContext context) : SqlRepository<Assignment>(context), IAssignmentRepository
    {
        public async Task<PagedList<Assignment>> GetAssignment(int page, int pageSize, string? search, bool isAscending)
        {
            var query = _dbSet.AsQueryable();
            if (!search.IsNullOrEmpty())
            {
                query = _dbSet.Where(x => x.Title.Contains(search) || x.Type.Contains(search) || x.Noticed.Contains(search));
            }
            if (isAscending)
            {
                query = query.OrderBy(x => x.CreatedAt);
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedAt);
            }
            return await query.ToPagedListAsync(page, pageSize);
        }

        public async Task<Assignment> GetAssignmentDetail(Guid assignmentId)
        {
            var query = _dbSet
            .AsNoTracking()
            .Where(x =>
                x.Id.Equals(assignmentId)
            )
            .Include(x => x.Submissions)
            .ThenInclude(s => s.Member)
            .Include(x => x.Questions)
            ;
            var result = await query.FirstOrDefaultAsync();
            return result;
        }
        public async Task<List<Assignment>> GetAssignmentByZoneId(Guid zoneId)
        {
            var query = _dbSet
            .AsNoTracking()
            .Where(x =>
                x.ZoneId.Equals(zoneId)
            ).Include(x => x.Submissions);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<Dictionary<Guid, int>> GetAssignmentCountByZoneId(Guid zoneId)
        {
            var assignments = await _dbSet
                .AsNoTracking()
                .Where(x => x.ZoneId == zoneId)
                .Include(x => x.Submissions)
                .ToListAsync();

            var submissionCount = assignments
                .ToDictionary(
                    assignment => assignment.Id,
                    assignment => assignment.Submissions.Count
                );

            return submissionCount;
        }
    }
}
