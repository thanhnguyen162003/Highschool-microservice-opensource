using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task<List<Document>> GetDocumentBySubjectIdFilter(List<Guid> subjectIds);
        Task<List<Document>> GetDocumentPlaceholder();
        Task<List<Document>> GetRelatedDocuments(Guid subjectId, CancellationToken cancellationToken = default);
        Task<Document> GetDocumentsByIds(Guid documentId, CancellationToken cancellationToken = default);
        Task<List<Document>> GetRelatedDocumentsByDocumentId(Guid documentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Document>> GetDocuments();
        Task<List<string>> GetDocumentIdsAsString(CancellationToken cancellationToken = default);
        Task<(List<Document> Documents, int TotalCount)> GetDocumentAdvanceFilter(DocumentAdvanceQueryFilter queryFilter, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetDocumentBySubjectId(IEnumerable<string> subjectIds, CancellationToken cancellationToken = default);
        Task<IEnumerable<Guid>> ExceptExistDocuments(IEnumerable<Guid> documentIds);
        Task<IEnumerable<Document>> GetDocumentByIds(IEnumerable<string> documentIds, CancellationToken cancellationToken = default);
    }
}
