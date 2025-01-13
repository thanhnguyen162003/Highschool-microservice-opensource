using Domain.Models.PlayGameModels;
using Infrastructure.Repositories.GenericRepository;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public class QuestionGameRepository : RedisRepository<QuestionGame>, IQuestionGameRepository
    {
        public QuestionGameRepository(IDatabase database, string keyPrefix) : base(database, keyPrefix)
        {
        }

        public async Task AddQuestions(string roomId, IEnumerable<QuestionGame> questions)
        {
            var tasks = questions.Select(async question =>
            {
                var redisKey = GetKey($"{roomId}:{question.Order}");
                var value = JsonSerializer.Serialize(question);

                await _database.StringSetAsync(redisKey, value);
            });


            await Task.WhenAll(tasks);
        }

        public async Task<QuestionGame?> GetQuestion(string roomId, int order)
        {
            var redisKey = GetKey($"{roomId}:{order}");
            var value = await _database.StringGetAsync(redisKey);

            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<QuestionGame>(value!);
        }

        public async Task DeleteQuestion(string roomId)
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

    }
}
