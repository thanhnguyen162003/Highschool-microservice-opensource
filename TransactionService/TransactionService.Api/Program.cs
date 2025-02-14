using Serilog;
using TransactionService.Api;

const string AppSettingsPath = "config/appsettings.json";

var config = new ConfigurationBuilder().AddJsonFile(AppSettingsPath, false, false).Build();
var staticLoggerConf = new LoggerConfiguration().ReadFrom.Configuration(config);
Log.Logger = staticLoggerConf.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Configuration.AddJsonFile(AppSettingsPath, optional: false);

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();
app.Run();