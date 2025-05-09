using Application.Services.CacheService.Interfaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Application.Services.CacheService;

public class RedisDistributedCache(IOptions<RedisCacheOptions> optionsAccessor, IConnectionMultiplexer connectionMultiplexer) : RedisCache(optionsAccessor), IRedisDistributedCache 
{
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;

    // Method to scan keys with a specific pattern
    public async IAsyncEnumerable<string> ScanAsync(string pattern)
    {
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
        foreach (var key in server.Keys(pattern: pattern))
        {
            yield return key;
        }
    }
}
