namespace TransactionService.Api.ServiceInterface;

[ApiController]
[Route("qry")]
public class OrganizationQueryController : QueryControllerBase
{
    readonly IOrganizationQueries Query;

    public OrganizationQueryController(IOrganizationQueries query, IQueryById queryById) : base(queryById)
    {
        Query = query;
    }

    [HttpPost("organizations")]
    public async Task<PaginatedResult<Organization>> Find(PaginatedQueryRequest req)
    {
        if (req.Qry.ContainsKey(QueryKeys.FindByIdKey))
            return await GetById<Organization>(req);
        else
            return await Query.Execute(req);
    }
}