namespace TransactionService.WebApi.ServiceInterface;

public class LookupService : Service
{
    readonly IQueryById QueryById;

    public LookupService(IQueryById queryById)
    {
        QueryById = queryById;
    }

    public async Task<object> Any(GetLookup req)
        => await QueryById.GetById<Lookup>(req.Id);
}