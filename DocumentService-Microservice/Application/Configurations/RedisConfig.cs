using Application.Services.CacheService;
using Application.Services.CacheService.Intefaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Application.Configurations;

public static class RedisConfig
{
    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration["Redis:RedisConfiguration"]));

        services.AddSingleton<IOrdinaryDistributedCache>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
            options.Configuration = configuration["Redis:RedisConfiguration"];
            var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return new OrdinaryDistributedCache(Options.Create(options), connectionMultiplexer);
        });
        services.AddDistributedMemoryCache();
    }
}

