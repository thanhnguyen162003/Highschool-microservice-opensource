using Serilog;

namespace TransactionService.WebApi;

public class Program
{
    const string AppSettingsPath = "config/appsettings.json";

    public static void Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddJsonFile(AppSettingsPath, false, false).Build();
        var staticLoggerConf = new LoggerConfiguration().ReadFrom.Configuration(config);
        Log.Logger = staticLoggerConf.CreateLogger();
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.Run();
    }
}