using Domain.Models.PlayGameModels;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface ILeaderboardRepository : IRedisRepository<PlayerGame>
    {
        Task AddPlayer(string roomId, PlayerGame player);
        int CountPlayer(string roomId);
        Task<IEnumerable<PlayerGame>> GetAll(string roomId);
        Task DeletePlayer(string roomId);
        Task Delete(string roomId, Guid userId);
        Task<PlayerGame?> UpdatePlayer(string roomId, Guid playerId, string? avatar, string? displayName);
        Task<PlayerGame?> UpdateProgress(string roomId, Guid playerId, int score, int time);
        Task Update(IEnumerable<PlayerGame> players, string roomId);
        Task<PlayerGame?> GetById(string roomId, Guid userId);
    }
}
