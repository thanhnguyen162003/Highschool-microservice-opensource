using Microsoft.Extensions.Caching.Distributed;

namespace Application.Common.Interfaces.CacheInterface
{
	public interface IOrdinaryDistributedCache : IDistributedCache
	{
		/// <summary>
		/// Scan the cache for keys matching the pattern.
		/// </summary>
		/// <param name="pattern">The pattern to match keys against.</param>
		/// <returns>An asynchronous enumerable of keys matching the pattern.</returns>
		/// <remarks>
		/// This method is not supported by all distributed cache implementations.
		/// </remarks>
		IAsyncEnumerable<string> ScanAsync(string pattern);
	}
}
