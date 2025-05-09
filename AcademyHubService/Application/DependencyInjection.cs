using Application.Features.Common;
using Application.Middlewares;
using Application.Services.Authentication;
using Application.Services.BackgroundServices.BackgroundTask;
using Application.Services.BackgroundServices.Workers;
using Application.Services.CalendarService;
using Application.Services.CalendarService.Interface;
using Application.Services.KafkaService.Producer;
using FluentValidation;
using Google.Api;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this WebApplicationBuilder builder)
        {
            // Set up global exception handler
            builder.Services.AddScoped<GlobalException>();

            // Set up AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Set up Validation
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });

            // Add Services
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<IProducerService, ProducerService>();
            builder.Services.AddScoped<ICalendarService, CalendarService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            builder.Services.AddHostedService<CommonWorkerService>();
        }
    }
}
