using Domain.Models.Common;
using Domain.Models.Settings;
using Infrastructure.Configuration;
using Infrastructure.Configurations;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this WebApplicationBuilder builder)
        {
            // Set up configuration
            builder.Services.Configure<DefaultSystem>(builder.Configuration.GetSection("DefaultSystem"));
            builder.Services.Configure<JWTSetting>(builder.Configuration.GetSection("JWTSetting"));
            builder.Services.Configure<AblySetting>(builder.Configuration.GetSection("AblySetting"));

            // Set up server
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 3 * 1024 * 1024; // 3GB
            });

            // Add PostgresSQL
            builder.Services.AddDbContext<CoolketContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"),
                    opt =>
                    {
                        opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        opt.UseRelationalNulls();
                    });
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Set up caching Redis
            builder.AddRedis();

            // Set up configuration Security
            builder.Services.AddJWT(builder.Configuration);

            // Set up Swagger
            builder.Services.AddSwagger();

            // Add Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddSingleton<ICacheRepository, CacheRepository>();

            // Anothers
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();
        }
    }
}
