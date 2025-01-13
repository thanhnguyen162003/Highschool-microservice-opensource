using Domain.Models.Game;
using Infrastructure.Repositories.GenericRepository;

namespace Infrastructure.Repositories
{
    public interface IRoomGameRepository : IRedisRepository<RoomGame>
    {
        Task<bool> IsExistRoom(string roomId);
    }
}
