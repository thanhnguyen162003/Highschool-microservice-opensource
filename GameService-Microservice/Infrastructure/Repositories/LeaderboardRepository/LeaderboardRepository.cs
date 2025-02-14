using Domain.Models.PlayGameModels;
using Infrastructure.Repositories.GenericRepository;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public class LeaderboardRepository : RedisRepository<PlayerGame>, ILeaderboardRepository
    {
        public LeaderboardRepository(IDatabase database, string keyPrefix) : base(database, keyPrefix)
        {
        }

        public async Task<PlayerGame?> GetById(string roomId, Guid userId)
        {
            var redisKey = GetKey($"{roomId}:{userId}");
            var value = await _database.StringGetAsync(redisKey);

            if(value.IsNullOrEmpty)
            {
                return null!;
            }

            return JsonSerializer.Deserialize<PlayerGame>(value!);
        }

        public async Task<IEnumerable<PlayerGame>> GetAll(string roomId)
        {
            var redisKey = GetKey($"{roomId}:*");
            var keys = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First()).Keys(pattern: redisKey);

            var players = new List<PlayerGame>();

            foreach (var key in keys)
            {
                var value = await _database.StringGetAsync(key);
                var player = JsonSerializer.Deserialize<PlayerGame>(value!);

                players.Add(player!);
            }

            return players.OrderByDescending(p => p.Score).ToList();
        }

        public async Task Delete(string roomId, Guid userId)
        {
            var redisKey = GetKey($"{roomId}:{userId.ToString()}");

            await _database.KeyDeleteAsync(redisKey);
        }

        public async Task AddPlayer(string roomId, PlayerGame player)
        {
            var redisKey = GetKey($"{roomId}:{player.Id}");
            var value = JsonSerializer.Serialize(player);

            await _database.StringSetAsync(redisKey, value);
        }

        public async Task Update(IEnumerable<PlayerGame> players, string roomId)
        {
            foreach (var player in players)
            {
                await AddPlayer(roomId, player); 
            }
        }

        public async Task DeletePlayer(string roomId)
        {
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{GetKey(roomId)}:*").ToArray();

            const int batchSize = 100;
            for (int i = 0; i < keys.Length; i += batchSize)
            {
                var batch = keys.Skip(i).Take(batchSize).ToArray();
                await _database.KeyDeleteAsync(batch);
            }
        }

        public async Task<PlayerGame?> UpdateProgress(string roomId, Guid playerId, int score, int time)
        {
            var redisKey = GetKey($"{roomId}:{playerId}");
            var value = await _database.StringGetAsync(redisKey);

            if (value.IsNullOrEmpty)
            {
                return null!;
            }

            var player = JsonSerializer.Deserialize<PlayerGame>(value!);

            player!.Score += score;
            player!.TimeAverage = (player.TimeAverage + time) / 2;

            await _database.StringSetAsync(redisKey, JsonSerializer.Serialize(player));

            return player;
        }

        public async Task<PlayerGame?> UpdatePlayer(string roomId, Guid playerId, string? avatar, string? displayName)
        {
            var redisKey = GetKey($"{roomId}:{playerId}");
            var value = await _database.StringGetAsync(redisKey);

            if (value.IsNullOrEmpty)
            {
                return null;
            }

            var player = JsonSerializer.Deserialize<PlayerGame>(value!);

            if (avatar != null)
            {
                player!.Avatar = avatar;
            }

            if (displayName != null)
            {
                player!.DisplayName = displayName;
            }

            await _database.StringSetAsync(redisKey, JsonSerializer.Serialize(player));

            return player;
        }

        public int CountPlayer(string roomId)
        {
            var redisKey = GetKey($"{roomId}:*");
            var keys = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First()).Keys(pattern: redisKey);

            var uniqueKeys = new HashSet<string>();

            foreach (var key in keys)
            {
                // Extract the identifier after ImageAI:
                var parts = key.ToString().Split(':');
                if (parts.Length > 1)
                {
                    uniqueKeys.Add(parts[1]);
                }
            }

            return uniqueKeys.Count();
        }
    }
}
