using Application.Common.Interfaces.CacheInterface;

namespace Application.Services.CacheService
{
	public class CleanCacheService(IOrdinaryDistributedCache cache) : ICleanCacheService
	{
		/// <summary>
		/// Clear University Global Cache
		/// </summary>
		public async Task ClearRelatedCacheUniversity()
		{
			var cachePatterns = new[] { "university:*", "universities:*" };

			foreach (var pattern in cachePatterns)
			{
				await foreach (var key in cache.ScanAsync(pattern))
				{
					await cache.RemoveAsync(key);
				}
			}
		}
		/// <summary>
		/// Clear University User Cache
		/// </summary>
		public async Task ClearRelatedCacheUniversitySave()
		{
			var cachePatterns = new[] {"universities:save:*" };

			foreach (var pattern in cachePatterns)
			{
				await foreach (var key in cache.ScanAsync(pattern))
				{
					await cache.RemoveAsync(key);
				}
			}
		}

	}
}
