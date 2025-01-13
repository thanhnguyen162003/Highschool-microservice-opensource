using Raven.Client.Documents;

namespace TransactionService.ReadModel.Queries.RavenDB;

public abstract class QueryBase<T>
{
    protected readonly IDocumentStore DocumentStore;

    public QueryBase(IDocumentStore documentStore)
    {
        DocumentStore = documentStore;
    }

    public async Task<PaginatedResult<T>> Execute(PaginatedQueryRequest qry)
    {
        QueryResult<T> qResult = await ExecuteAsync(qry);
        var resp = ToPaginatedResult(qry, qResult);
        if (CurrentPageIsOverflown(resp))
            return await Execute(
                new PaginatedQueryRequest() { Qry = qry.Qry, CurrentPage = 0, PageSize = qry.PageSize });
        return resp;
    }

    protected abstract Task<QueryResult<T>> ExecuteAsync(PaginatedQueryRequest qry);

    protected PaginatedResult<T> ToPaginatedResult(PaginatedQueryRequest request, QueryResult<T> qr)
    {
        PaginatedResult<T> retVal = new PaginatedResult<T>() { Data = new List<T>() };
        retVal.Data = qr.Data;
        retVal.TotalItems = qr.Statistics.TotalResults;
        retVal.TotalPages = retVal.TotalItems / request.PageSize;
        if ((retVal.TotalItems % request.PageSize) > 0)
            retVal.TotalPages += 1;
        retVal.PageSize = request.PageSize;
        retVal.CurrentPage = request.CurrentPage;
        return retVal;
    }

    static bool CurrentPageIsOverflown(PaginatedResult<T> result)
        => (result.Data.Count == 0) && (result.TotalPages > 0);

    protected string GetParamValue(PaginatedQueryRequest req, string key)
    {
        return req.Qry.ContainsKey(key) ? req.Qry[key] : string.Empty;
    }
}