using Application.Common.Behaviours;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Ultils;
using Application.Common.UoW;
using Application.Infrastructure;
using Application.Services;
using Infrastructure.Data.Interceptors;
using Infrastructure.Repositories.Interfaces;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Application.Features.NotificationFeature;
using Application.Services.CacheService;
using Application.Services.CacheService.Intefaces;
using Microsoft.AspNetCore.SignalR;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        //Inject Service, Repo, etc...
        services.AddSingleton<IClaimInterface, ClaimService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IProducerService, ProducerService>();
        services.AddSingleton<ICurrentTime, CurrentTime>();
        services.AddScoped<ICacheOption, CacheOption>();
        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddScoped(typeof(ValidationHelper<>));

        //hosted service
        services.AddHostedService<NotificationDocumentConsumer>();
        services.AddHostedService<NewsNotificationConsumer>();
        services.AddHostedService<SystemNotificationConsumer>();
        services.AddHostedService<NotificationUserConsumer>();
        services.AddHostedService<FlashcardNotificationConsumer>();
        services.AddHostedService<FlashcardAINotificationConsumer>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationService API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        return services;
    }

}