using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;

namespace TransactionService.ReadModel.Queries.RavenDB;

public class OrganizationQueries : QueryBase<Organization>, IOrganizationQueries, ITypeaheadQuery
{
    public OrganizationQueries(IDocumentStore documentStore) : base(documentStore)
    {
    }

    protected override async Task<QueryResult<Organization>> ExecuteAsync(PaginatedQueryRequest req)
    {
        var result = new QueryResult<Organization>();
        var stats = new QueryStatistics();
        using (var ses = DocumentStore.OpenAsyncSession())
        {
            var searchResult = await QueryData(req, out stats, ses)
                .Skip(req.CurrentPage * req.PageSize)
                .Take(req.PageSize)
                .OfType<Organization>()
                .ToListAsync();

            if (searchResult != null)
                result.Data = searchResult;

            result.Statistics = stats;
        }

        return result;
    }

    IQueryable<Organizations_Search.Result> QueryData(PaginatedQueryRequest req, out QueryStatistics qryStats,
        IAsyncDocumentSession ses)
    {
        return ses.Query<Organizations_Search.Result, Organizations_Search>()
            .Statistics(out qryStats).Search(x => x.Query,
                GetParamValue(req, QueryKeys.SearchKey),
                @operator: Raven.Client.Documents.Queries.SearchOperator.And
            );
    }

    async Task<PaginatedResult<RefEx>> ITypeaheadQuery.Execute(PaginatedQueryRequest req)
    {
        var res = await Execute(req);
        var lng = GetParamValue(req, QueryKeys.LanguageKey);
        return CreateFrom(res, res.Data.Select(x => x.CovertToTypeaheadItem(lng)).ToList());
    }

    static PaginatedResult<RefEx> CreateFrom(IPaginatedResult src, List<RefEx> data)
    {
        return new PaginatedResult<RefEx>
        {
            CurrentPage = src.CurrentPage,
            PageSize = src.PageSize,
            TotalItems = src.TotalItems,
            TotalPages = src.TotalPages,
            Data = data
        };
    }
}

public class Organizations_Search : AbstractIndexCreationTask<Organization>
{
    //ncrunch: no coverage start
    public class Result
    {
        public string[] Query { get; set; }
    }
    //ncrunch: no coverage end

    public Organizations_Search()
    {
        Map = organizations => from c in organizations
            select new Result
            {
                Query = new string[] { c.Name }
            };

        Index("Query", FieldIndexing.Search);
    }
}