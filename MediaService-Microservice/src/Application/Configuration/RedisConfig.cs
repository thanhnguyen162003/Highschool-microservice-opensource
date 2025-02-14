

using Application.Services.CacheService;
using Application.Services.CacheService.Interfaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Application.Configuration;

public static class RedisConfig
{
    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration["Redis:RedisConfiguration"]));

        services.AddSingleton<IRedisDistributedCache>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
            options.Configuration = configuration["Redis:RedisConfiguration"];
            var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return new RedisDistributedCache(Options.Create(options), connectionMultiplexer);
        });
        services.AddDistributedMemoryCache();
    }
}
