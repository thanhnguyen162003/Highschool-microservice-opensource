using Raven.Client.Documents;

namespace TransactionService.ReadModel.Queries.RavenDB;

public class QueryById : IQueryById
{
    protected readonly IDocumentStore DocumentStore;

    public QueryById(IDocumentStore documentStore)
    {
        DocumentStore = documentStore;
    }

    public async Task<T> GetById<T>(string id)
    {
        using (var ses = DocumentStore.OpenAsyncSession())
        {
            return await ses.LoadAsync<T>(id);
        }
    }

    public async Task<Dictionary<string, T>> GetByIds<T>(IEnumerable<string> ids)
    {
        using (var ses = DocumentStore.OpenAsyncSession())
        {
            var res = await ses.LoadAsync<T>(ids);
            return res;
        }
    }
}