using Domain.Models.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Configurations
{
    public static class RedisConfig
    {
        public static void AddRedis(this WebApplicationBuilder builder)
        {
            var redisSetting = builder.Configuration.GetSection("RedisSetting").Get<RedisSetting>() ?? throw new Exception("Redis Setting is not config!");

            var options = new ConfigurationOptions
            {
                ClientName = redisSetting.ClientName,
                EndPoints = { { redisSetting.Host, redisSetting.Port } },
                User = redisSetting.User,
                Password = redisSetting.Password,
                AbortOnConnectFail = false
            };

            var multiplexer = ConnectionMultiplexer.Connect(options);
            builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        }
    }
}
