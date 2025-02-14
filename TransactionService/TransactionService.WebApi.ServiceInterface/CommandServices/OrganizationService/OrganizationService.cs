namespace TransactionService.WebApi.ServiceInterface;

public class OrganizationService : DomainCommandService
{
    public OrganizationService(IMessageBus bus, ITimeProvider timeProvider) : base(bus, timeProvider)
    {
    }

    public async Task<object> Any(RegisterOrganization request)
        => await TryAutoMapAndProcessRequest<PL.Commands.RegisterOrganization>(request);
}