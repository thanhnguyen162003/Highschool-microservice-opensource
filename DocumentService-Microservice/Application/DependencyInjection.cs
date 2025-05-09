using Application.Common.Behaviours;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.AIInferface;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models.QuestionModel;
using Application.Common.Models.UserQuizProgress;
using Application.Common.StartupTask;
using Application.Common.Ultils;
using Application.Common.UoW;
using Application.Configurations;
using Application.Consumers;
using Application.Features.DocumentFeature.EventHandler;
using Application.Features.DocumentFeature.EventHandlers;
using Application.Features.FlashcardFeature.EventHandler;
using Application.Features.LessonFeature.Events;
using Application.Features.QuestionFeature.Validations;
using Application.Features.SearchFeature.EventHandler;
using Application.Features.SubjectFeature.EventHandler;
using Application.Features.UserQuizProgressFeature.Validators;
using Application.Infrastructure;
using Application.Services;
using Application.Services.AIService;
using Application.Services.CacheService;
using Application.Services.CacheService.Intefaces;
using Application.Services.CalendarService;
using Application.Services.CalendarService.Interface;
using Application.Services.SearchService;
using Application.WorkerService;
using Infrastructure.Dapper;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddScoped<GlobalException>();
        services.AddScoped<IClaimInterface, ClaimService>();
		services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
		services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IProducerService, ProducerService>();
        services.AddSingleton<ICurrentTime, CurrentTime>();
        services.AddScoped<ICacheOption, CacheOption>();
        services.AddScoped<ICleanCacheService, CleanCacheService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<IFlashcardStudyService, FlashcardStudyService>();
        services.AddScoped<IFlashcardTestService, FlashcardTestService>();
        services.AddExceptionHandler<CustomExceptionHandler>();
        // services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<IAlgoliaService, AlgoliaService>();
		services.AddScoped<IAIService, AIService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IEventService, EventService>();
        
        services.AddScoped<IValidator<QuestionRequestModel>, QuestionCommandValidator>();
        services.AddScoped<IValidator<List<QuestionRequestModel>>, QuestionListCommandValidator>();
        services.AddScoped<IValidator<SubmitAnswerRequestModel>, SubmitAnswerRequestValidator>();
        services.AddScoped<IValidator<ClearUserQuizProgressRequestModel>, ClearUserQuizProgressValidator>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        


        services.AddScoped(typeof(ValidationHelper<>));
        services.AddStreamingServices();

		services.AddHostedService<ConsumerUpdateSubjectService>();
        services.AddHostedService<FlashcardNotificationWorker>();
        services.AddHostedService<ConsumerUpdateDocumentService>();
        services.AddHostedService<DataRecommendedConsumer>();
		services.AddHostedService<LessonVideoConsumer>();
        services.AddHostedService<ConsumerUpdateSubjectViewService>();
        services.AddHostedService<ConsumerUpdateDocumentViewService>();
        services.AddHostedService<ConsumerUpdateFlashcardViewService>();
        services.AddHostedService<ConsumerUpdateFlashcardVoteService>();
        services.AddHostedService<ConsumerDataSearchModified>();
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
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "DocumentService API", Version = "v1" });
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