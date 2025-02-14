namespace TransactionService.WebApi.ServiceInterface;

public class TypeaheadQueryService : Service
{
    readonly ITypeaheadQueries Query;

    public TypeaheadQueryService(ITypeaheadQueries query)
        => Query = query;

    public async Task<object> Any(FilterTypeahead req)
        => await Query.Execute(req);
}