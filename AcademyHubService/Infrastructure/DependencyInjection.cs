using Domain.Models.Common;
using Domain.Models.Settings;
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
            builder.Services.Configure<WorkerSetting>(builder.Configuration.GetSection("WorkerSetting"));
            builder.Services.Configure<KafkaSetting>(builder.Configuration.GetSection("KafkaSetting"));

            // Set up server
            var setting = builder.Configuration.GetSection("DefaultSystem").Get<DefaultSystem>();
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = setting?.LimitSizeFile; // 3GB
            });

            // Add PostgresSQL
            builder.Services.AddDbContext<AcademyHubContext>(options =>
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

            // Set up policy
            builder.Services.AddPolicies();

            // Set up Swagger
            builder.Services.AddSwagger();

            // Add Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Set up grpc
            //builder.Services.AddGrpcConfig(builder.Configuration);

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
