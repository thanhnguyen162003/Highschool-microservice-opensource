using DStack.Projections;

namespace TransactionService.ReadModel.Impl;

public interface ILookupsInitializer
{
    Task Initialize();
}

public class LookupsInitializer : ILookupsInitializer
{
    INoSqlStore Store;

    public LookupsInitializer(INoSqlStore store)
    {
        Store = store;
    }

    public async Task Initialize()
    {
        await CreateTimezones();
        await CreateLanguages();
    }

    async Task CreateTimezones()
    {
        const string DocumentId = "lookups-timezones";
        List<Ref> data = (from t in TimeZoneInfo.GetSystemTimeZones() select new Ref { Id = t.Id, Val = t.DisplayName })
            .ToList();
        await CreateLookupDocument(DocumentId, data);
    }

    async Task CreateLanguages()
    {
        const string DocumentId = "lookups-languages";
        List<Ref> data = new List<Ref>()
        {
            new Ref { Id = "en", Val = "English" },
            new Ref { Id = "de", Val = "German" }
        };

        await CreateLookupDocument(DocumentId, data);
    }

    async Task CreateLookupDocument(string DocumentId, List<Ref> data)
    {
        var doc = await Store.LoadAsync<Lookup>(DocumentId);
        if (null != doc)
            return;
        doc = new Lookup { Id = DocumentId, Data = data };
        await Store.StoreAsync(doc);
    }
}