using System.Reflection;
using Application.Common.Behaviours;
using Application.Common.Ultils;
using Application.Common.Interfaces.AWS3ServiceInterface;
using Application.Common.Interfaces.ClaimInterface;
using Application.Common.Interfaces.CloudinaryInterface;
using Application.Common.Interfaces.KafkaInterface;
using Application.Common.Models.NewsModel;
using Application.Features.NewsFeature.Validations;
using Application.Features.SubjectFeature.Events;
using Application.Infrastructure;
using Application.Services;
using Infrastructure.Data;
using Microsoft.OpenApi.Models;
using Application.Common.Models.NewsTagModel;
using Application.Features.NewsTagFeature.Validations;
using Application.Features.NewsFeature.EventHandler;
using Application.Services.AIService;
using Application.Common.Interfaces.AzureInterface;
using Application.Services.AzureService;
using Azure.Storage.Blobs;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddHostedService<SubjectCreateConsumer>();

        //Inject Service, Repo, etc...
        services.AddSingleton<MediaDbContext>();
        services.AddScoped<IClaimInterface, ClaimService>();
        services.AddSingleton<IProducerService, ProducerService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>(); 
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<IAWSS3Service, AWSS3Service>();
        services.AddScoped<IAIService, AIService>();
        services.AddSingleton(TimeProvider.System);
        // services.AddScoped<ImageToPdfHelper>();

        //validator
        //services.AddScoped<IValidator<DiscussCreateRequestModel>, CreateDiscussCommandValidator>();
        //services.AddScoped<IValidator<DiscussUpdateRequestModel>, UpdateDiscussCommandValidator>();

        //services.AddScoped<IValidator<TagCreateRequestModel>, CreateTagCommandValidator>();
        //services.AddScoped<IValidator<TagUpdateRequestModel>, UpdateTagCommandValidator>();
        //services.AddScoped<IValidator<List<TagCreateRequestModel>>, CreateTagListCommandValidator>();

        //services.AddScoped<IValidator<CommentCreateRequestModel>, CreateCommentCommandValidator>();
        //services.AddScoped<IValidator<CommentUpdateRequestModel>, UpdateCommentCommandValidator>();

        //services.AddScoped<IValidator<UpVoteCreateDiscussionRequestModel>, CreateUpVoteDiscussionCommandValidator>();
        //services.AddScoped<IValidator<UpVoteCreateCommentRequestModel>, CreateUpVoteCommentCommandValidator>();

        services.AddScoped<IValidator<NewsCreateRequestModel>, CreateNewsCommandValidator>();
        services.AddScoped<IValidator<NewsUpdateRequestModel>, UpdateNewsCommandValidator>();
        services.AddScoped<IValidator<NewsTagCreateRequestModel>, CreateNewsTagCommandValidator>();
        services.AddScoped<IValidator<List<NewsTagCreateRequestModel>>, CreateNewsTagListCommandValidator>();
        services.AddScoped<IValidator<NewsTagUpdateRequestModel>, UpdateNewsTagCommandValidator>();

        services.AddScoped(typeof(ValidationHelper<>));

        services.AddHostedService<ConsumerUpdateNewViewService>();

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
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "MediaService API", Version = "v1" });
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
