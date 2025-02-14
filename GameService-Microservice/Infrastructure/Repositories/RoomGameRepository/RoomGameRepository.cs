using Domain.Models.Game;
using Infrastructure.Repositories.GenericRepository;
using StackExchange.Redis;

namespace Infrastructure.Repositories
{
    public class RoomGameRepository : RedisRepository<RoomGame>, IRoomGameRepository
    {
        public RoomGameRepository(IDatabase database, string keyPrefix) : base(database, keyPrefix)
        {
        }

        public async Task<bool> IsExistRoom(string roomId)
        {
            var rooms = await GetById(roomId);

            return rooms != null;

        }


    }
}
