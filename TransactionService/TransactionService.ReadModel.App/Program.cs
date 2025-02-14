using DStack.Projections;
using DStack.Projections.EventStoreDB;
using DStack.Projections.EventStoreDB.Utils;
using DStack.Projections.RavenDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Linq;
using System.Reflection;
using TransactionService.ReadModel.Impl;
using TransactionService.ReadModel.Projections;
using TransactionService.ReadModel.Queries.RavenDB;

namespace TransactionService.ReadModel.App;

class Program
{
    async static Task Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddJsonFile(Consts.AppSettingsPath, false, false).Build();
        var staticLoggerConf = new LoggerConfiguration().ReadFrom.Configuration(config);
        Log.Logger = staticLoggerConf.CreateLogger();
        await CreateHostBuilder(args).Build().RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                var docStore =
                    new RavenDocumentStoreFactory().CreateAndInitializeDocumentStore(
                        RavenConfig.FromConfiguration(hostContext.Configuration));
                services.AddSingleton(docStore);
                services.AddSingleton<INoSqlStore, DefensiveRavenDBProjectionsStore>();
                services.AddSingleton<ISqlStore, DefensiveRavenDBProjectionsStore>();
                services.AddTransient<IQueryById, QueryById>();
                services.AddTransient<ICheckpointReader, RavenDBCheckpointReader>();
                services.AddTransient<ICheckpointWriter, RavenDBCheckpointWriter>();
                services.AddTransient<IHandlerFactory, DIHandlerFactory>();
                services.AddTransient<ISubscriptionFactory, ESSubscriptionFactory>();
                services.AddTransient<IProjectionsFactory, ProjectionsFactory>();
                services.AddTransient<IJSProjectionsFactory, JSProjectionsFactory>();
                services.AddTransient<ILookupsInitializer, LookupsInitializer>();

                RegisterProjectionHandlers(services);

                services.AddHostedService<ServiceInstance>();
            })
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile("config/appsettings.json", optional: false);
                configHost.AddEnvironmentVariables(prefix: "STARNET_");
                configHost.AddCommandLine(args);
            }).UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });

    static void RegisterProjectionHandlers(IServiceCollection services)
    {
        var a = Assembly.GetAssembly(typeof(OrganizationProjectionHandler));
        var results = from type in a.GetTypes()
            where typeof(IHandler).IsAssignableFrom(type)
            select type;
        foreach (var t in results)
            services.AddTransient(t);
    }
}