using Application.Services.CacheService.Intefaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Application.Services.CacheService;

public class OrdinaryDistributedCache : RedisCache, IOrdinaryDistributedCache 
{   
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    public OrdinaryDistributedCache(IOptions<RedisCacheOptions> optionsAccessor, IConnectionMultiplexer connectionMultiplexer) 
        : base(optionsAccessor)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

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