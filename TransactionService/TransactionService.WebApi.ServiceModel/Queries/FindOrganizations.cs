namespace TransactionService.WebApi.ServiceModel;

[Route("/qry/organizations", Verbs = "POST")]
public record FindOrganizations : PaginatedQueryRequest, IReturn<PaginatedResult<Organization>>
{
}