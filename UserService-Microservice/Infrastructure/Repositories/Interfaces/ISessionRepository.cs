using Domain.Entities;
using Infrastructure.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<Dictionary<string, int>> GetSessionStatisticToday();
        //Task<List<Session>?> GetSessionStatisticWeek(int amount);
        //Task<List<Session>?> GetSessionStatisticMonth(int amount);
        //Task<List<Session>?> GetSessionStatisticYear(int amount);
        Task<List<UserLoginStatisticModel>> GetUserLoginToday();
        Task<List<SessionStatisticModel>> GetUserGrowth(DateTime start, DateTime end);
        Task<List<SessionStatisticModel>> GetUserGrowthForYear(DateTime start, DateTime end);
        Task<List<SessionStatisticModel>> GetUserActivity(DateTime start, DateTime end);
        Task<List<SessionStatisticModel>> GetUserActivityForYear(DateTime start, DateTime end);
        Task<Dictionary<string, int>> GetSessionStatisticYear(int amount);
        Task<Session?> GetSession(string refreshToken, string sessionId);
        Task<Session?> GetSessionUser(Guid sessionId, Guid userId);
        Task DeleteSession(Guid userId);
    }
}
