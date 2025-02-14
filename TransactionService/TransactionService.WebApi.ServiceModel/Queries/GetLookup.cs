namespace TransactionService.WebApi.ServiceModel;

[Route("/lookups")]
public class GetLookup : IReturn<Lookup>
{
    public string Id { get; set; }
}