namespace TransactionService.WebApi.ServiceInterface;

public class QueryServiceBase : Service
{
    protected readonly IQueryById QueryById;

    public QueryServiceBase(IQueryById queryById)
    {
        QueryById = queryById;
    }

    protected async Task<PaginatedResult<T>> GetById<T>(PaginatedQueryRequest req)
    {
        var c = await QueryById.GetById<T>(req.Qry[QueryConsts.Keys.ById]);
        return c == null ? new PaginatedResult<T>() : PaginatedResult<T>.CreateFrom(c);
    }

    protected async Task<PaginatedResult<T>> GetByIds<T>(PaginatedQueryRequest req)
    {
        var c = await QueryById.GetByIds<T>(req.Qry[QueryConsts.Keys.ByIds].Split(';'));
        return c == null
            ? new PaginatedResult<T>()
            : new PaginatedResult<T>()
            {
                PageSize = c.Count, TotalItems = c.Count, CurrentPage = 0, TotalPages = 1,
                Data = c.Values.Where(x => x != null).ToList()
            };
    }

    protected PaginatedResult<T> Transform<K, T>(PaginatedResult<K> inp, Func<K, T> transform)
    {
        return new PaginatedResult<T>()
        {
            CurrentPage = inp.CurrentPage,
            PageSize = inp.PageSize,
            TotalItems = inp.TotalItems,
            TotalPages = inp.TotalPages,
            Data = inp.Data.Select(x => transform(x)).ToList()
        };
    }
}