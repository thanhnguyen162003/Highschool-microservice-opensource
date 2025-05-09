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
        Task<List<string>> GetDocumentBySubjectId(List<Guid> subjectIds, CancellationToken cancellationToken = default);
        Task<IEnumerable<Guid>> ExceptExistDocuments(IEnumerable<Guid> documentIds);
        Task<List<Document>> GetDocumentByIds(IEnumerable<string> documentIds, CancellationToken cancellationToken = default);

        //recommended
        Task<List<Document>> GetDocumentsByMasterSubjectId(Guid masterSubjectId, int? grade, CancellationToken cancellationToken);
        Task<List<Document>> GetDocumentPlaceholder(int? grade = null);
        Task<List<Document>> GetDocumentPlaceholderNoGrade(int numberMissing);
        Task<int> GetDocumentsCount(CancellationToken cancellationToken = default);
        Task<Dictionary<DateTime, int>> GetDocumentsCountByTime(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<Dictionary<DateTime, int>> GetDocumentsCountByDay(DateTime start, DateTime? end, CancellationToken cancellationToken = default);
    }
}
