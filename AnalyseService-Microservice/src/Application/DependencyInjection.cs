using System.Reflection;
using Application.Common.Behaviours;
using Application.Common.Interfaces.AWS3ServiceInterface;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Interfaces.FlashcardAnalyzeServiceInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models;
using Application.Common.Models.RoadmapDataModel;
using Application.Common.Ultils;
using Application.Consumer;
using Application.Consumer.RetryConsumer;
using Application.Features.AnalyseFeature.EventHandler;
using Application.Features.RoadmapFeature.Validators;
using Application.Infrastructure;
using Application.Services;
using Application.Services.FlashcardAnalyze;
using Application.Services.Search;
using Infrastructure.Data;
using Microsoft.OpenApi.Models;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddHostedService<UserDataAnalyseConsumer>();
        services.AddHostedService<UserRoadmapGenConsumer>();
        services.AddHostedService<UserDataAnalyseRetryConsumer>();
        services.AddHostedService<UserRoadmapGenRetryConsumer>();
        services.AddHostedService<FlashcardAnalyzeConsumer>();
        services.AddHostedService<RecentViewKafkaConsumer>();
        services.AddHostedService<DailyTaskService>();
        services.AddHostedService<FiveMinutesTaskService>();
        services.AddHostedService<RecentViewDeleteConsumer>();
        //services.AddHostedService<RoadmapMissedMaintainService>();
        //Inject Service, Repo, etc...
        services.AddSingleton<AnalyseDbContext>();
        services.AddScoped<IClaimInterface, ClaimService>();
        services.AddSingleton<IProducerService, ProducerService>();
        services.AddSingleton<IProducerBatchService, ProducerBatchService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddScoped<IAWSS3Service, AWSS3Service>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IFlashcardAnalyzeService, FlashcardAnalyzeService>();
        services.AddScoped<IFlashcardFormattingService, FlashcardFormattingService>();
        
        //validator
        services.AddScoped<IValidator<RoadMapSectionCreateRequestModel>, CreateRoadmapSectionCommandValidator>();
        services.AddScoped<IValidator<RoadmapCreateRequestModel>, CreateRoadmapCommandValidator>();
        services.AddScoped(typeof(ValidationHelper<>));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

}
