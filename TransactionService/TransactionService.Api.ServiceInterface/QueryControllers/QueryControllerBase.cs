namespace TransactionService.Api.ServiceInterface;

public class QueryControllerBase : ControllerBase
{
    protected readonly IQueryById QueryById;

    public QueryControllerBase(IQueryById queryById)
    {
        QueryById = queryById;
    }

    protected async Task<PaginatedResult<T>> GetById<T>(PaginatedQueryRequest req)
    {
        var c = await QueryById.GetById<T>(req.Qry[QueryConsts.Keys.ById]);
        return c == null ? new PaginatedResult<T>() : PaginatedResult<T>.CreateFrom(c);
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