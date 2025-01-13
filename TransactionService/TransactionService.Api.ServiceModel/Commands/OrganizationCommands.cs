namespace TransactionService.Api.ServiceModel.Commands;

public class RegisterOrganization
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
}