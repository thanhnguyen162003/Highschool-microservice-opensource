using Application.Common.Interfaces.CacheInterface;
using Application.Service.Cloudinary;
using Application.Services;
using Application.Services.CacheService;
using Application.Services.Cloudinary;
using Application.Services.EventHandler;
using Application.Services.EventHandler.RetryConsumer;
using Application.Services.MailService;
using Application.Services.MasterData;
using Application.Services.ServiceTask.Common;
using Application.Services.Workers;
using Application.TransactionalOutbox;
using Domain.Common.Behaviours;
using Domain.Common.Interfaces.ClaimInterface;
using Domain.Common.Interfaces.KafkaInterface;
using Domain.Configuration;
using Domain.Features.Common;
using Domain.Services;
using Domain.Services.Authentication;
using Domain.Services.BackgroundTask;
using Domain.Services.ServiceTask.CacheCommon;
using Domain.Services.Workers;
using FAI.API.Middleware;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Domain;

public static class DependencyInjection
{
	public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
	{
		// Set up global exception handling
		services.AddScoped<GlobalException>();

		// Add services
		services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
		services.AddScoped<IClaimInterface, ClaimService>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddSingleton<IProducerService, ProducerService>();
		services.AddScoped<IAuthenticationService, AuthenticationService>();
		services.AddScoped<IEmailService, EmailService>();
		services.AddScoped<ICloudinaryService, CloudinaryService>();
		services.AddScoped<IOpenAIService, OpenAIService>();
		services.AddScoped<ICleanCacheService, CleanCacheService>();

		services.AddSingleton<ICacheRepository, CacheRepository>();
		services.AddSingleton<CareerMongoDatabaseContext>();

		// Set up task for background task
		services.AddSingleton<ICacheDataTask, CacheDataTask>();
		services.AddSingleton<ICommonTask, CommonTask>();
		services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
		services.AddSingleton<IMasterDataService, MasterDataService>();

		// Set up workers
		services.AddHostedService<CacheWorkerService>();
		services.AddHostedService<CommonWorkerService>();
        services.AddHostedService<ConsumerProgressStageUpdate>();
		services.AddHostedService<ConsumerSubjectCreate>();
		services.AddHostedService<ConsumerSubjectDelete>();
		services.AddHostedService<ConsumerMail>();
		services.AddHostedService<ConsumerMailZone>();
		services.AddHostedService<OutboxMessageProcessor>();

		// Set up Validation
		services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

		// Add MediatR
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			cfg.RegisterServicesFromAssemblyContaining<Program>();
			cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
			cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
		});

		// Set up swagger
		services.AddSwagger();

        return services;
	}

}