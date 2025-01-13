namespace TransactionService.ReadModel;

public interface ITypeaheadQueries
{
    Task<PaginatedResult<RefEx>> Execute(PaginatedQueryRequest qry);
}

public interface ITypeaheadQuery
{
    Task<PaginatedResult<RefEx>> Execute(PaginatedQueryRequest qry);
}