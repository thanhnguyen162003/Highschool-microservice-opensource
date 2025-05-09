using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.CacheService.Intefaces;

public interface IOrdinaryDistributedCache : IDistributedCache
{
    IAsyncEnumerable<string> ScanAsync(string pattern);
}