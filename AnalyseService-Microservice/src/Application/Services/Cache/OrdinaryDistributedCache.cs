using Application.Services.CacheService.Intefaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace Application.Services.CacheService;

public class OrdinaryDistributedCache : RedisCache, IOrdinaryDistributedCache 
{
    public OrdinaryDistributedCache(IOptions<RedisCacheOptions> optionsAccessor) : base(optionsAccessor)
    {
    }
}