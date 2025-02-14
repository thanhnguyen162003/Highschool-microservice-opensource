using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface ITheoryRepository : IRepository<Theory>
{
    Task<bool> SoftDelete(Guid theoryId, CancellationToken cancellationToken = default);
    Task<bool> CreateTheory(Theory theory, CancellationToken cancellationToken = default);
    Task<bool> UpdateTheory(Theory theory, CancellationToken cancellationToken = default);
    Task<(List<Theory> Theories, int TotalCount)> GetTheoryByFilters(Guid? lessonId, TheoryQueryFilter queryFilter, CancellationToken cancellationToken = default);
    Task<bool> TheoryIdExistAsync(Guid theoryId, CancellationToken cancellationToken = default);
    Task<Theory> GetTheoryByIdAsync(Guid theoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Theory>> GetTheoryForTips(IEnumerable<string> theoryIds, CancellationToken cancellationToken = default);
}