using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.CacheService.Interfaces;

public interface IRedisDistributedCache : IDistributedCache
{
    IAsyncEnumerable<string> ScanAsync(string pattern);
}
