using Application.Services.CacheService;
using Application.Services.CacheService.Intefaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace Application.Configurations;

public static class RedisConfig
{
    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {

        // Configuration for Ordinary Cache
        services.AddSingleton<IOrdinaryDistributedCache>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
            options.Configuration = configuration["Redis:RedisConfiguration"];
            return new OrdinaryDistributedCache(Options.Create(options));
        });
        // Optionally, add in-memory cache
        services.AddDistributedMemoryCache();
    }
}


