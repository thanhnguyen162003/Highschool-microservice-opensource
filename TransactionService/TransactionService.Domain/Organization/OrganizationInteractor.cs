namespace TransactionService.Domain.Organization;

public interface IOrganizationInteractor : IInteractor
{
}

public class OrganizationInteractor : Interactor<OrganizationAggregate>, IOrganizationInteractor
{
    public OrganizationInteractor(IAggregateRepository aggRepository)
    {
        AggregateRepository = aggRepository;
    }

    public override async Task ExecuteAsync(object command)
        => await When((dynamic)command);

    async Task When(RegisterOrganization c)
        => await IdempotentlyCreateAgg(c.Id, agg => agg.RegisterOrganization(c));

    async Task When(CorrectOrganizationName c)
        => await IdempotentlyUpdateAgg(c.Id, agg => agg.CorrectOrganizationName(c));
}