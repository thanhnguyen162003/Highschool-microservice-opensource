namespace TransactionService.WebApi.ServiceInterface;

public class OrganizationQueryService : QueryServiceBase
{
    readonly IOrganizationQueries Query;

    public OrganizationQueryService(IOrganizationQueries query, IQueryById queryById) : base(queryById)
    {
        Query = query;
    }

    public async Task<object> Any(FindOrganizations req)
    {
        if (req.Qry.ContainsKey(QueryKeys.FindByIdKey))
            return await GetById(req);
        else
            return await Query.Execute(req);
    }

    async Task<object> GetById(FindOrganizations req)
    {
        var c = await QueryById.GetById<Organization>(req.Qry[QueryKeys.FindByIdKey]);
        return c == null
            ? new PaginatedResult<Organization>()
            : new PaginatedResult<Organization>()
            {
                PageSize = 1, TotalItems = 1, CurrentPage = 0, TotalPages = 1, Data = new List<Organization>() { c }
            };
    }
}