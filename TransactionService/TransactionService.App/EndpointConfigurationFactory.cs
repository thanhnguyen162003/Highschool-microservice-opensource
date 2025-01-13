using Microsoft.Extensions.Configuration;
using NServiceBus;
using DStack.Aggregates.EventStoreDB;
using EventStore.Client;
using DStack.Aggregates;
using Microsoft.Extensions.DependencyInjection;
using TransactionService.ReadModel.Queries.RavenDB;

namespace TransactionService.App;

class EndpointConfigurationFactory
{
    public EndpointConfiguration Create(IConfiguration config)
    {
        var endpointConfiguration = new EndpointConfiguration(config["NSBus:EndpointName"]);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.LicensePath("config/license.xml");
        RegisterComponents(config, endpointConfiguration);
        InitializeTransport(config, endpointConfiguration);
        InitializeRavenDBPersistence(config, endpointConfiguration);
        SetupConventions(endpointConfiguration);
        SetupHeartBeatAndMetrics(config, endpointConfiguration);
        SetupAuditing(config, endpointConfiguration);
        endpointConfiguration.EnableInstallers();
        return endpointConfiguration;
    }

    void RegisterComponents(IConfiguration config, EndpointConfiguration endpointConfiguration)
        => endpointConfiguration.RegisterComponents(reg =>
        {
            var esAggRep = CreateEventStoreAggregateRepository(config);
            reg.AddSingleton<IAggregateRepository>(esAggRep);

            reg.AddTransient<ITimeProvider, StarnetTimeProvider>();
            RegisterAggregateInteractors(reg);
        });

    ESAggregateRepository CreateEventStoreAggregateRepository(IConfiguration config)
    {
        var settings = EventStoreClientSettings.Create(config["EventStoreDB:ConnectionString"]);
        var client = new EventStoreClient(settings);
        AssertEventStoreAvailable(client);
        return new ESAggregateRepository(client);
    }

    void AssertEventStoreAvailable(EventStoreClient client)
        => _ = client.GetStreamMetadataAsync("$ce-Any").Result;

    static void RegisterAggregateInteractors(IServiceCollection reg)
    {
        foreach (var type in AggregateInteractorsExtractor.GetInteractors())
            reg.AddTransient(type.Key, type.Value);
    }

    static void InitializeTransport(IConfiguration config, EndpointConfiguration endpointConfiguration)
    {
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.UseConventionalRoutingTopology(QueueType.Classic);
        transport.ConnectionString(config["RabbitMQ:ConnectionString"]);
    }

    static void InitializeRavenDBPersistence(IConfiguration config, EndpointConfiguration endpointConfiguration)
    {
        var persistence = endpointConfiguration.UsePersistence<RavenDBPersistence>();
        var docStore =
            new RavenDocumentStoreFactory().CreateAndInitializeDocumentStore(
                RavenConfig.FromConfiguration(config, "RavenDBForNSBus"));
        persistence.SetDefaultDocumentStore(docStore);
    }


    static void SetupConventions(EndpointConfiguration endpointConfiguration)
    {
        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(type => type.Namespace == "TransactionService.PL.Commands");
        conventions.DefiningEventsAs(type => type.Namespace == "TransactionService.PL.Events");
        conventions.DefiningMessagesAs(type => type.Namespace == "TransactionService.PL.Messages");
    }

    static void SetupAuditing(IConfiguration config, EndpointConfiguration endpointConfiguration)
    {
        var isTurnedOn = config["NSBus:Audit"];
        if (!bool.Parse(isTurnedOn))
            return;
        endpointConfiguration.AuditProcessedMessagesTo("audit");
    }

    static void SetupHeartBeatAndMetrics(IConfiguration config, EndpointConfiguration endpointConfiguration)
    {
        var isTurnedOn = config["NSBus:HeartbeatAndMetrics"];
        if (!bool.Parse(isTurnedOn))
            return;

        var svcControlInstanceName = config["NSBus:ServiceControlInstanceName"];
        if (!string.IsNullOrWhiteSpace(svcControlInstanceName))
        {
            endpointConfiguration.SendHeartbeatTo(
                serviceControlQueue: svcControlInstanceName,
                frequency: TimeSpan.FromSeconds(15),
                timeToLive: TimeSpan.FromSeconds(30));
        }

        var monInstName = config["NSBus:MonitoringInstanceName"];
        if (!string.IsNullOrWhiteSpace(monInstName))
        {
            var metrics = endpointConfiguration.EnableMetrics();
            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress: monInstName,
                interval: TimeSpan.FromSeconds(10),
                instanceId: config["NSBus:EndpointName"]
            );
        }
    }
}