using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IStudiableTermRepository : IRepository<StudiableTerm>
    {
        Task<bool> CreateStudiableTermList(List<StudiableTerm> studiableTerm, CancellationToken cancellation);
        Task<bool> DeleteStudiableTerm(List<StudiableTerm> studiableTerms, CancellationToken cancellationToken);
        Task<List<StudiableTerm>> GetStudiableTerm(Guid userId, Guid contentId);
        Task<bool> UpdateStudiableTermList(List<StudiableTerm> studiableTerm, CancellationToken cancellation);
        Task<List<StudiableTerm>> CheckDuplicateStudiableTerm(Guid userId, List<Guid> flashcardContentIds, Guid contentId);
        Task<bool> CreateStudiableTerm(StudiableTerm studiableTerm, CancellationToken cancellationToken);
        Task<bool> UpdateStudiableTerm(StudiableTerm studiableTerm, CancellationToken cancellation);
    }
}
