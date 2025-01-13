using Application.Common.Behaviours;
using Application.Common.Interfaces;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Ultils;
using Application.Common.UoW;
using Application.Consumers;
using Application.Features.DocumentFeature.EventHandlers;
using Application.Features.SubjectFeature.EventHandler;
using Application.Infrastructure;
using Application.Services;
using Infrastructure.Data.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Application.Common.Exceptions;
using Application.Services.CacheService.Intefaces;
using Application.Services.CacheService;
using Application.Features.LessonFeature.Events;
using Application.Features.DocumentFeature.EventHandler;
using Application.Common.Models.QuestionModel;
using Application.Features.QuestionFeature.Validations;
using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models.UserQuizProgress;
using Application.Features.UserQuizProgressFeature.Validators;
using Application.Features.FlashcardFeature.EventHandler;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddScoped<GlobalException>();
        //Inject Service, Repo, etc...
        services.AddScoped<IClaimInterface, ClaimService>();
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
        services.AddScoped<AuditableEntityInterceptor>();

        //validator
        // services.AddScoped<IValidator<CurriculumCreateRequestModel>, CurriculumCreateValidation>();
        // services.AddScoped<IValidator<SubjectCreateRequestModel>, CreateSubjectCommandValidator>();
        // services.AddScoped<IValidator<SubjectModel>, UpdateSubjectCommandValidator>();
        // services.AddScoped<IValidator<FlashcardCreateRequestModel>, CreateFlashcardCommandValidator>();
        // services.AddScoped<IValidator<FlashcardUpdateRequestModel>, UpdateFlashcardCommandValidator>();
        // services.AddScoped<IValidator<FlashcardContentCreateRequestModel>, CreateFlashcardContentCommandValidator>();
        // services.AddScoped<IValidator<List<FlashcardContentCreateRequestModel>>, CreateFlashcardContentListValidator>();
        // services.AddScoped<IValidator<FlashcardContentUpdateRequestModel>, FlashcardContentUpdateRequestModelValidator>();
        // services.AddScoped<IValidator<List<FlashcardContentUpdateRequestModel>>, UpdateFlashcardContentListValidator>();
        // services.AddScoped<IValidator<ChapterCreateRequestModel>, CreateChapterCommandValidator>();
        // services.AddScoped<IValidator<List<ChapterCreateRequestModel>>, CreateChapterListCommandValidator>();
        // services.AddScoped<IValidator<ChapterUpdateRequestModel>, UpdateChapterCommandValidator>();
        // services.AddScoped<IValidator<List<ChapterUpdateRequestModel>>, UpdateChapterListCommandValidator>();
        // services.AddScoped<IValidator<LessonCreateRequestModel>, LessonCreateCommandValidator>();
        // services.AddScoped<IValidator<List<LessonCreateRequestModel>>, LessonCreateListCommandValidator>();
        // services.AddScoped<IValidator<LessonUpdateRequestModel>, LessonUpdateCommandValidator>();
        // services.AddScoped<IValidator<List<LessonUpdateRequestModel>>, LessonUpdateListCommandValidator>();
        // services.AddScoped<IValidator<CreateDocumentRequestModel>, CreateDocumentCommandValidator>();
        // services.AddScoped<IValidator<UpdateDocumentRequestModel>, UpdateDocumentCommandValidator>();
        // services.AddScoped<IValidator<TheoryCreateRequestModel>, TheoryCreateCommandValidator>();
        services.AddScoped<IValidator<QuestionRequestModel>, QuestionCommandValidator>();
        services.AddScoped<IValidator<List<QuestionRequestModel>>, QuestionListCommandValidator>();
        services.AddScoped<IValidator<SubmitAnswerRequestModel>, SubmitAnswerRequestValidator>();
        services.AddScoped<IValidator<ClearUserQuizProgressRequestModel>, ClearUserQuizProgressValidator>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        


        services.AddScoped(typeof(ValidationHelper<>));

        //add consume service
        services.AddHostedService<ConsumerUpdateSubjectService>();
        services.AddHostedService<ConsumerUpdateDocumentService>();
        services.AddHostedService<DataRecommendedConsumer>();
		services.AddHostedService<LessonVideoConsumer>();
        //services.AddHostedService<AlgoliaInitData>();
        services.AddHostedService<ConsumerUpdateSubjectViewService>();
        services.AddHostedService<ConsumerUpdateDocumentViewService>();
        services.AddHostedService<ConsumerUpdateFlashcardViewService>();
        services.AddHostedService<ConsumerUpdateFlashcardVoteService>();
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