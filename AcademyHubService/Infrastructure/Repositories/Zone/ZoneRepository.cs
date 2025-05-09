using Domain.Entity;
using Domain.Models.Common;
using Infrastructure.Extensions;
using Infrastructure.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories
{
    public class ZoneRepository(DbContext context) : SqlRepository<Zone>(context), IZoneRepository
    {
        public async Task<int> GetZoneCreationCount()
        {
            var query = await _dbSet.Where(x => x.DeletedAt == null)
                .GroupBy(x => x.CreatedBy)
                .CountAsync();
            return query;
        }
        public async Task<PagedList<Zone>> GetAllZone(int page, int pageSize, string? search, bool isAscending)
        {
            var query = _dbSet
                .Include(x => x.ZoneMemberships)
                .Include(x => x.Assignments)
                .AsQueryable();
            if (!search.IsNullOrEmpty())
            {
                query = _dbSet.Where(x => x.Name.Contains(search) || x.Description.Contains(search));
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
        public async Task<PagedList<Zone>> GetAllZoneForStudent(int page, int pageSize, string? search, bool isAscending, Guid userId)
        {
            var query = _dbSet
                .Include(x => x.ZoneMemberships)
                .Include(x => x.Assignments)
                .Where(x => x.ZoneMemberships.Any(z =>z.UserId == userId))
                .AsQueryable();
            if (!search.IsNullOrEmpty())
            {
                query = _dbSet.Where(x => x.Name.Contains(search) || x.Description.Contains(search));
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
        public async Task<PagedList<Zone>> GetAllZoneForTeacher(int page, int pageSize, string? search, bool isAscending, Guid userId, IEnumerable<Guid> zoneIds)
        {
            var query1 = _dbSet
                .Where(x => x.CreatedBy.Equals(userId))
                .Include(x => x.ZoneMemberships)
                .Include(x => x.Assignments);

            var query2 = _dbSet
                .Where(x => zoneIds.Contains(x.Id))
                .Include(x => x.ZoneMemberships)
                .Include(x => x.Assignments);

            var query = query1.Union(query2).AsQueryable();

            if (!search.IsNullOrEmpty())
            {
                query = _dbSet.Where(x => x.Name.Contains(search) || x.Description.Contains(search));
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
        public async Task<Zone> GetZoneDetail(Guid zoneId)
        {
            var query = _dbSet
            .AsNoTracking()
            .Where(x =>
                x.Id.Equals(zoneId) &&
                x.DeletedAt == null
            )
            .Include(x => x.Assignments)
            .Include(x => x.ZoneBans)
            .Include(x => x.PendingZoneInvites)
            .Include(x => x.ZoneMemberships);
            var result = await query.FirstOrDefaultAsync();
            return result;
        }
        public async Task<Dictionary<string, int>> GetZoneSubmissionScore(Guid zoneId)
        {
            var query = await _dbSet
            .AsNoTracking()
            .Where(x =>
                x.Id.Equals(zoneId) &&
                x.DeletedAt == null
            )
            .Include(x => x.Assignments)
            .ThenInclude(x => x.Submissions)
            .FirstOrDefaultAsync();

            if (query == null)
                return new Dictionary<string, int>()
                {
                    { "0-3", 0 },
                    { "3-5", 0 },
                    { "5-6.5", 0 },
                    { "6.5-8", 0 },
                    { "8+", 0 }
                };

            var ranges = new Dictionary<string, Func<double, bool>>
            {
                { "0-3", score => score >= 0 && score < 3 },
                { "3-5", score => score >= 3 && score < 5 },
                { "5-6.5", score => score >= 5 && score < 6.5 },
                { "6.5-8", score => score >= 6.5 && score < 8 },
                { "8+", score => score >= 8 }
            };

            var result = ranges.ToDictionary(r => r.Key, r => 0);

            var scores = query.Assignments
                .SelectMany(a => a.Submissions)
                .Where(s => s.Score.HasValue)
                .Select(s => s.Score.Value);

            foreach (var score in scores)
            {
                foreach (var range in ranges)
                {
                    if (range.Value(score))
                    {
                        result[range.Key]++;
                        break;
                    }
                }
            }


            return result;
        }

    }
}

