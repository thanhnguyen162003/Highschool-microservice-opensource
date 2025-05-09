using Domain.Settings;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Repositories
{
	public class CacheRepository : ICacheRepository
	{
		private readonly IDistributedCache _cache;
		private readonly DefaultSystem _default;

		private readonly JsonSerializerSettings jsonOpt;

		private readonly DistributedCacheEntryOptions cacheOpt;

		public CacheRepository(IDistributedCache distributedCache, IOptions<DefaultSystem> options)
		{
			_cache = distributedCache;
			_default = options.Value;
			cacheOpt = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(_default.CacheTime) // Set cache time 1d
			};
			jsonOpt = new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
		}

		public async Task<T> GetAsync<T>(string key)
		{
			var cacheValue = await _cache.GetStringAsync(key);

			// If cache has value, return cache data
			if (!string.IsNullOrEmpty(cacheValue))
			{
				return JsonConvert.DeserializeObject<T>(cacheValue!)!;
			}

			return default!;
		}

		public async Task<T> GetAsync<T>(string type, string key)
		{
			var fullKey = $"{type}:{key}";

			var cacheValue = await _cache.GetStringAsync(fullKey);

			// If cache has value, return cache data
			if (!string.IsNullOrEmpty(cacheValue))
			{
				return JsonConvert.DeserializeObject<T>(cacheValue!)!;
			}

			return default!;
		}

		public async Task RemoveAsync(string key)
		{
			await _cache.RemoveAsync(key);
		}

		public async Task RemoveAsync(string type, string key)
		{
			var fullKey = $"{type}:{key}";

			await _cache.RemoveAsync(fullKey);
		}

		public async Task SetAsync<T>(string key, T value)
		{

			var jsonOpt = new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};

			await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value, jsonOpt), cacheOpt);

		}

		public async Task SetAsync<T>(string type, string key, T value)
		{
			var fullKey = $"{type}:{key}";

			await _cache.SetStringAsync(fullKey, JsonConvert.SerializeObject(value, jsonOpt), cacheOpt);

		}

		public async Task SetAsync<T>(string type, string key, T value, int timeCache)
		{
			var fullKey = $"{type}:{key}";


			var jsonOpt = new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};

			var customOpt = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timeCache) // Set cache time 1M
			};

			await _cache.SetStringAsync(fullKey, JsonConvert.SerializeObject(value, jsonOpt), customOpt);

		}



	}
}
