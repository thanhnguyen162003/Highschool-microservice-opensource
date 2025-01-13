namespace TransactionService.ReadModel.Queries.RavenDB;

public static class TypeaheadConsts
{
    public const string CollectionKey = "collection";
    public const string SearchParamKey = "search";

    public const string OrganizationsCollection = "organizations";
}

public class TypeaheadQueries : ITypeaheadQueries
{
    IOrganizationQueries OrganizationQueries;

    public TypeaheadQueries(IOrganizationQueries organizationQueries)
    {
        OrganizationQueries = organizationQueries;
    }

    public async Task<PaginatedResult<RefEx>> Execute(PaginatedQueryRequest req)
    {
        return await GetQueryByCollection(req).Execute(req);
    }

    ITypeaheadQuery GetQueryByCollection(PaginatedQueryRequest req)
    {
        if (req.Qry.ContainsKey(TypeaheadConsts.CollectionKey))
        {
            var collection = req.Qry[TypeaheadConsts.CollectionKey];

            switch (collection)
            {
                case TypeaheadConsts.OrganizationsCollection:
                    return (ITypeaheadQuery)OrganizationQueries;

                default:
                    throw new NotImplementedException();
            }
        }

        throw new NotImplementedException();
    }
}