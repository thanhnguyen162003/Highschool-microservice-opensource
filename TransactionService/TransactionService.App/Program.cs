using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Extensions.Logging;
using NServiceBus.Logging;
using Serilog;
using System.IO;

namespace TransactionService.App;

partial class Program
{
    const string AppSettingsPath = "config/appsettings.json";
    const string SerilogEmailSinkSection = "SerilogEmailSink";

    async static Task Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddJsonFile(AppSettingsPath, false, false).Build();
        var staticLoggerConf = new LoggerConfiguration().ReadFrom.Configuration(config);
        Log.Logger = staticLoggerConf.CreateLogger();
        await CreateHostBuilder(args).Build().RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile("config/appsettings.json", optional: false);
                configHost.AddEnvironmentVariables(prefix: "STARNET_");
                configHost.AddCommandLine(args);
            }).UseNServiceBus(hostBuilderContext =>
                new EndpointConfigurationFactory().Create(hostBuilderContext.Configuration)
            ).UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });
}