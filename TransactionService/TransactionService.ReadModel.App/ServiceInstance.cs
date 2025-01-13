using TransactionService.ReadModel.Projections;
using DStack.Projections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DStack.Projections.EventStoreDB.Utils;
using TransactionService.ReadModel.Projections.EventStoreDB;
using TransactionService.ReadModel.Impl;

namespace TransactionService.ReadModel.App;

internal class ServiceInstance : IHostedService
{
    readonly ILogger<ServiceInstance> Logger;
    readonly IConfiguration Configuration;
    readonly IProjectionsFactory ProjectionsFactory;
    readonly IJSProjectionsFactory JSProjectionsFactory;
    readonly ILookupsInitializer LookupsInitializer;

    public ServiceInstance(ILogger<ServiceInstance> logger, IConfiguration configuration,
        IProjectionsFactory projectionsFactory, IJSProjectionsFactory jSProjectionsFactory,
        ILookupsInitializer lookupsInitializer)
    {
        Logger = logger;
        Configuration = configuration;
        ProjectionsFactory = projectionsFactory;
        JSProjectionsFactory = jSProjectionsFactory;
        LookupsInitializer = lookupsInitializer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await LookupsInitializer.Initialize();
        await CreateEventStoreProjections();
        await RunProjections();
    }

    async Task CreateEventStoreProjections()
    {
        foreach (var v in EventStoreJavaScriptProjectionDefinitionsBuilder.Build())
            JSProjectionsFactory.AddProjection(v.Key, v.Value);
        await JSProjectionsFactory.CreateProjections();
    }

    async Task RunProjections()
    {
        var projections = await ProjectionsFactory.CreateAsync(Assembly.GetAssembly(typeof(OrganizationProjection)));
        await RunProjections(projections, GetEnabledProjctions(projections), GetDisabledProjections());
    }

    string[] GetEnabledProjctions(IList<IProjection> projections)
    {
        var enabledProjectionsSetting = Configuration["Projections:Enabled"];
        if (enabledProjectionsSetting.ToLower() == "all")
            return GetProjectionNames(projections);
        else
            return enabledProjectionsSetting.Split(";");
    }

    string[] GetProjectionNames(IList<IProjection> projections)
    {
        var data = new List<string>();
        foreach (var p in projections)
            data.Add(p.Name);
        return data.ToArray();
    }

    string[] GetDisabledProjections()
    {
        var disabledProjectionsSetting = Configuration["Projections:Disabled"];
        if (disabledProjectionsSetting.ToLower() == "none")
            return new string[0];
        else
            return disabledProjectionsSetting.Split(";");
    }

    async Task RunProjections(IList<IProjection> projections, string[] enabledProjections, string[] disabledProjections)
    {
        foreach (var p in projections)
        {
            if (enabledProjections.Contains(p.Name) && (!disabledProjections.Contains(p.Name)))
            {
                Logger.LogInformation($"Starting projection {p.Name} on stream {p.Subscription.StreamName}.");
                _ = p.StartAsync();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}