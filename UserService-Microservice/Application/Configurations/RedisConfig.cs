using Application.Common.Interfaces.CacheInterface;
using Application.Services.CacheService;
using StackExchange.Redis;

namespace Domain.Configurations
{
	public static class RedisConfig
	{
		public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
		{
			// Get the Redis configuration string
			var redisConnectionString = configuration["Redis:RedisConfiguration"];

			// Remove clientName parameter for AddStackExchangeRedisCache which doesn't support it
			string distributedCacheConnectionString = RemoveClientNameParameter(redisConnectionString);

			// Configure Redis distributed cache
			services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = distributedCacheConnectionString;
			});

			// Add memory cache for fallback/local cache
			services.AddDistributedMemoryCache();

			// For ConnectionMultiplexer, we can use the full connection string with clientName
			services.AddSingleton<IConnectionMultiplexer>(sp =>
			{
				// For ConnectionMultiplexer, we use ConfigurationOptions to set all parameters properly
				var configOptions = ConfigurationOptions.Parse(redisConnectionString);
				return ConnectionMultiplexer.Connect(configOptions);
			});

			// Register custom cache interface for scanning support
			services.AddSingleton<IOrdinaryDistributedCache, OrdinaryDistributedCache>();
		}

		// Helper method to remove the clientName parameter from the connection string
		private static string RemoveClientNameParameter(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
				return connectionString;

			// Split by commas, filter out any clientName parameter, then rejoin
			var parameters = connectionString.Split(',');
			var filteredParameters = parameters.Where(p => !p.Trim().StartsWith("clientName=", StringComparison.OrdinalIgnoreCase));
			return string.Join(',', filteredParameters);
		}
	}
}