using Domain.Models.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _cache;
        private readonly DefaultSystem _default;

        private JsonSerializerSettings jsonOpt = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };


        public CacheRepository(IConnectionMultiplexer connectionMultiplexer, IOptions<DefaultSystem> options)
        {
            _cache = connectionMultiplexer.GetDatabase();
            _default = options.Value;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var cacheValue = await _cache.StringGetAsync(key);

            // If cache has value, return cache data
            if (!cacheValue.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<T>(cacheValue!)!;
            }

            return default!;
        }

        public async Task<T> GetAsync<T>(string type, string key)
        {
            var fullKey = $"{type}:{key}";

            var cacheValue = await _cache.StringGetAsync(fullKey);

            // If cache has value, return cache data
            if (!cacheValue.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<T>(cacheValue!)!;
            }

            return default!;
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public async Task RemoveAsync(string type, string key)
        {
            var fullKey = $"{type}:{key}";

            await _cache.KeyDeleteAsync(fullKey);
        }

        public async Task SetAsync<T>(string key, T value)
        {

            var jsonOpt = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            await _cache.StringSetAsync(key, JsonConvert.SerializeObject(value, jsonOpt), TimeSpan.FromMinutes(_default.CacheTime));

        }

        public async Task SetAsync<T>(string type, string key, T value)
        {
            var fullKey = $"{type}:{key}";

            await _cache.StringSetAsync(fullKey, JsonConvert.SerializeObject(value, jsonOpt), TimeSpan.FromMinutes(_default.CacheTime));

        }

        public async Task SetAsync<T>(string type, string key, T value, int timeCache)
        {
            var fullKey = $"{type}:{key}";


            var jsonOpt = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            await _cache.StringSetAsync(fullKey, JsonConvert.SerializeObject(value, jsonOpt), TimeSpan.FromMinutes(timeCache));

        }

    }
}
