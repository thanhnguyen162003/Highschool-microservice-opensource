namespace TransactionService.Api.ServiceInterface;

[ApiController]
[Route("typeaheads")]
public class TypeaheadQueryController : ControllerBase
{
    readonly ITypeaheadQueries Query;

    public TypeaheadQueryController(ITypeaheadQueries query)
    {
        Query = query;
    }

    [HttpPost]
    public async Task<PaginatedResult<RefEx>> Find(PaginatedQueryRequest req)
    {
        return await Query.Execute(req);
    }
}