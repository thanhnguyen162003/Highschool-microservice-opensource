using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using Infrastructure.CustomEntities;
using Infrastructure.Repositories.Interfaces;
using MongoDB.Driver.Linq;
using static MongoDB.Driver.WriteConcern;

namespace Infrastructure.Repositories
{
    public class SessionRepository(UserDatabaseContext context) : BaseRepository<Session>(context), ISessionRepository
    {
        private readonly UserDatabaseContext _context = context;

        public async Task<Session?> GetSession(string refreshToken, string sessionId)
        {
            return await _context.Sesssions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.RefreshToken.Equals(refreshToken) && s.Id.ToString().Equals(sessionId));
        }

        public async Task<List<UserLoginStatisticModel>> GetUserLoginToday()
        {
            var utcPlus7Zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // UTC+7
            var now = TimeZoneInfo.ConvertTime(DateTime.Now, utcPlus7Zone).Date;
            var results = await _entities
                .Where(x => x.UpdatedAt >= new DateTime(now.Year, now.Month, now.Day, 0, 0, 0) && x.UpdatedAt <= new DateTime(now.Year, now.Month, now.Day, 23, 59, 59) && x.CreatedAt.Date == new DateTime(now.Year, now.Month, now.Day).Date)
                .Include(s => s.User)
                .GroupBy(s => s.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastLogin = g.Max(s => s.UpdatedAt),
                    Role = g.FirstOrDefault().User != null ? g.FirstOrDefault().User.RoleId.ToString() : "Unknown"
                }).ToListAsync();

            return results.Select(g => new UserLoginStatisticModel
            {
                UserId = g.UserId.ToString(),
                Date = g.LastLogin,
                Role = g.Role
            }).ToList();
        }
        public async Task<Dictionary<string, int>> GetSessionStatisticToday()
        {
            var target = DateTime.Now.Date;
            var results = await _entities
            .Where(x => x.UpdatedAt >= new DateTime(target.Year, target.Month, target.Day, 0, 0, 0) && x.UpdatedAt <= new DateTime(target.Year, target.Month, target.Day, 23, 59, 59))
            .Include(s => s.User)
            .ThenInclude(u => u.Role)  
            .GroupBy(s => s.User.Role.RoleName) 
            .Select(g => new
            {
                RoleName = g.Key,   
                Count = g.Select(s => s.UserId).Distinct().Count()
            })
            .ToListAsync();

            return results.ToDictionary(
            x => x.RoleName ?? "Unknown",
                x => x.Count
            );
        }
        public async Task<Dictionary<string, int>> GetSessionStatisticYear(int amount)
        {
            DateTime start = DateTime.Now.AddYears(amount -1);
            DateTime end = DateTime.Now.AddYears(amount);
            
            var results = await _entities
                .Where(s => s.UpdatedAt >= start && s.UpdatedAt < end).OrderBy(s => s.UpdatedAt)
                .Include(s => s.User)
                .ThenInclude(u => u.Role)  // Correct include for navigation property
                .GroupBy(s => s.User.Role.RoleName)  // Group by role name
                .Select(g => new
                {
                    RoleName = g.Key,   // string
                    Count = g.Select(s => s.UserId).Distinct().Count()   // int
                })
                .ToListAsync();

            // Convert to Dictionary<string, int>
            return results.ToDictionary(
                x => x.RoleName ?? "Unknown",  // Handle null RoleName
                x => x.Count
            );
        }

        public async Task<List<SessionStatisticModel>> GetUserActivityForYear(DateTime start , DateTime end)
        {
            var groupedData = await _entities
            .Where(s => s.UpdatedAt >= start && s.UpdatedAt < end)
            .GroupBy(s => new { Year = s.UpdatedAt.Year, Month = s.UpdatedAt.Month, Role = s.User.Role.RoleName })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                Role = g.Key.Role,
                Count = g.Select(s => s.UserId).Distinct().Count()
            })
            .ToListAsync();

            var result = groupedData
                .GroupBy(g => g.Date)
                .Select(g => new SessionStatisticModel
                {
                    Date = g.Key,
                    Data = g.ToDictionary(x => x.Role, x => x.Count)
                })
                .ToList();

            return result;
        }

        public async Task<List<SessionStatisticModel>> GetUserGrowthForYear(DateTime start, DateTime end)
        {
            var groupedData = await _entities
            .Where(s => s.CreatedAt >= start && s.CreatedAt < end)
            .GroupBy(s => new { Year = s.CreatedAt.Year, Month = s.CreatedAt.Month, Role = s.User.Role.RoleName })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                Role = g.Key.Role,
                Count = g.Select(s => s.UserId).Distinct().Count()
            })
            .ToListAsync();

            var result = groupedData
                .GroupBy(g => g.Date)
                .Select(g => new SessionStatisticModel
                {
                    Date = g.Key,
                    Data = g.ToDictionary(x => x.Role, x => x.Count)
                })
                .ToList();

            return result;
        }
        public async Task<List<SessionStatisticModel>> GetUserGrowth(DateTime start, DateTime end)
        {
            var groupedData = await _entities
            .Where(s => s.CreatedAt >= start && s.CreatedAt < end)
            .GroupBy(s => new { Year = s.CreatedAt.Year, Month = s.CreatedAt.Month, Day = s.CreatedAt.Day, Role = s.User.Role.RoleName })
            .OrderBy(g => g.Key.Month).ThenBy(g => g.Key.Day)
            .Select(g => new
            {
                Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                Role = g.Key.Role,
                Count = g.Select(s => s.UserId).Distinct().Count()
            })
            .ToListAsync();

            var result = groupedData
                .GroupBy(g => g.Date)
                .Select(g => new SessionStatisticModel
                {
                    Date = g.Key,
                    Data = g.ToDictionary(x => x.Role, x => x.Count)
                })
                .ToList();

            return result;
        }
        public async Task<List<SessionStatisticModel>> GetUserActivity(DateTime start, DateTime end)
        {
            var groupedData = await _entities
            .Where(s => s.UpdatedAt >= start && s.UpdatedAt < end)
            .GroupBy(s => new { Year = s.UpdatedAt.Year   ,Month = s.UpdatedAt.Month, Day = s.UpdatedAt.Day, Role = s.User.Role.RoleName })
            .OrderBy(g => g.Key.Month).ThenBy(g => g.Key.Day)
            .Select(g => new
            {
                Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                Role = g.Key.Role,
                Count = g.Select(s => s.UserId).Distinct().Count()
            })
            .ToListAsync();

            var result = groupedData
                .GroupBy(g => g.Date)
                .Select(g => new SessionStatisticModel
                {
                    Date = g.Key,
                    Data = g.ToDictionary(x => x.Role, x => x.Count)
                })
                .ToList();

            return result;
        }
        //public async Task<Dictionary<string, int>> GetSessionStatisticMonth(int amount)
        //{

        //    DateTime start = DateTime.Now.AddMonths(amount-1);
        //    DateTime end = DateTime.Now.AddMonths(amount);


        //    var results = await _entities
        //        .AsNoTracking() // Improves read performance
        //        .Where(s => s.UpdatedAt >= start && s.UpdatedAt < end).OrderBy(s => s.UpdatedAt)
        //        .GroupBy(s => s.User.Role.RoleName)
        //        .Select(g => new { RoleName = g.Key ?? "Unknown", Count = g.Count() })
        //        .ToDictionaryAsync(x => x.RoleName, x => x.Count); // Avoids intermediate list

        //    return results;
        //}
        //public async Task<List<Session>?> GetSessionStatisticWeek(int amount)
        //{
        //    DateTime start;
        //    DateTime end;
        //    if (amount == 0)
        //    {
        //        start = DateTime.Now.AddDays(-7);
        //        end = DateTime.Now;
        //    }
        //    else
        //    {
        //        start = DateTime.Now.AddDays((double)(-7 * amount));
        //        end = DateTime.Now.AddDays((double)(-7 * (amount - 1)));
        //    }

        //    return await _entities.Where(s => s.UpdatedAt >= start && s.UpdatedAt < end).OrderBy(s => s.UpdatedAt).ToListAsync();
        //}
        //public async Task<List<Session>?> GetSessionStatisticMonth(int amount)
        //{
        //    var target = DateTime.Now.AddMonths(amount).Month;
        //    var results = await _entities
        //        .Where(s => s.UpdatedAt.Month == target).OrderBy(s => s.UpdatedAt)
        //        .ToListAsync();
        //    return results;
        //}
        //public async Task<List<Session>?> GetSessionStatisticYear(int amount)
        //{
        //    DateTime start;
        //    DateTime end;
        //    if (amount == 0)
        //    {
        //        start = DateTime.Now.AddYears(-1);
        //        end = DateTime.Now;
        //    }
        //    else
        //    {
        //        start = DateTime.Now.AddYears(-1 * amount);
        //        end = DateTime.Now.AddYears(-1 * (amount - 1));
        //    }
        //    var results = await _entities
        //        .Where(s => s.UpdatedAt >= start && s.UpdatedAt < end).OrderBy(s => s.UpdatedAt)
        //        .ToListAsync();
        //    return results;
        //}
        public async Task<Session?> GetSessionUser(Guid sessionId, Guid userId)
        {
            return await _context.Sesssions.FirstOrDefaultAsync(s => s.Id.Equals(sessionId) && s.UserId.Equals(userId));
        }

        public async Task DeleteSession(Guid userId)
        {
            await _entities.Where(s => s.UserId.Equals(userId)).ForEachAsync(s => _entities.Remove(s));
        }
    }
}
