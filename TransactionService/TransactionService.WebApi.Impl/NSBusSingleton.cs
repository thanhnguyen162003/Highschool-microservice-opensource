using Microsoft.Extensions.Configuration;
using NServiceBus;

namespace TransactionService.WebApi.Impl;

public class NSBus : IMessageBus
{
    public Task Publish(object message)
    {
        throw new NotImplementedException();
    }

    public async Task Send(object message)
        => await NSBusSingleton.AppEndpointInstance.Send(message);
}

class NSBusSingleton
{
    public static IEndpointInstance AppEndpointInstance;

    static NSBusSingleton()
    {
        AppEndpointInstance = Endpoint.Start(CreateEndpointConfiguration()).GetAwaiter().GetResult();
    }

    static EndpointConfiguration CreateEndpointConfiguration()
    {
        var config = new ConfigurationBuilder().AddJsonFile("config/appsettings.json", true, true).Build();
        var endpointConfiguration = new EndpointConfiguration(config["NSBus:EndpointName"]);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.LicensePath("config/license.xml");

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.UseConventionalRoutingTopology(QueueType.Classic);

        transport.ConnectionString(config["RabbitMQ:ConnectionString"]);

        var routing = transport.Routing();
        routing.RouteToEndpoint(
            assembly: typeof(PL.Commands.RegisterOrganization).Assembly,
            destination: config["NSBus:AppEndpointName"]);

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(
            type =>
                type.Namespace == "TransactionService.PL.Commands"
        );

        endpointConfiguration.SendOnly();
        endpointConfiguration.EnableInstallers();
        return endpointConfiguration;
    }
}