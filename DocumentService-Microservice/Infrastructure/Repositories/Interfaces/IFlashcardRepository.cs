using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface IFlashcardRepository : IRepository<Flashcard>
{
    Task<IEnumerable<Flashcard>> GetFlashcards(FlashcardQueryFilter queryFilter);
    Task<IEnumerable<Flashcard>> GetFlashcardsPlaceholder();
    Task<IEnumerable<Flashcard>> GetTopFlashcard();
	Task<IEnumerable<Flashcard>> EveryOneAlsoWatch();
	Task<IEnumerable<Flashcard>> GetFlashcardsWithToken(FlashcardQueryFilter queryFilter, Guid userId);
    Task<Flashcard> GetFlashcardById(Guid flashcardId);
    Task<Flashcard> GetFlashcardByIdNoStatus(Guid flashcardId);
    Task<Flashcard> GetFlashcardByIdWithToken(Guid flashcardId, Guid? userId);
    Task<List<Flashcard>> GetFlashcardBySubjectIdFilter(List<Guid> subjectIds);
    Task<IEnumerable<Flashcard>> GetFlashcardsByUserId(FlashcardQueryFilter queryFilter, Guid userId);
    Task<int> CheckNumberFlashcardInUser(Guid userId);
    Task<IEnumerable<Flashcard>> GetFlashcardByUsername(FlashcardQueryFilter queryFilter, string username);
    Task<Flashcard> GetFlashcardDraftById(Guid flashcardId, Guid userId);
    Task<Flashcard> GetFlashcardByUserId(Guid userId);
    Task<IEnumerable<Flashcard>> GetFlashcardsBySubject(FlashcardQueryFilter queryFilter, Guid id);
    Task<IEnumerable<Flashcard>> GetFlashcardsBySubjectId(Guid subjectId, CancellationToken cancellationToken = default);
    Task<Flashcard> GetFlashcardDraftByUserId(Guid userId);
    Task<bool> CreateFlashcard(Flashcard flashcard);
    Task<bool> UpdateFlashcard(Flashcard flashcard, Guid userId);
    Task<bool> UpdateCreatedFlashcard(Flashcard flashcard, Guid id);
    Task<bool> DeleteFlashcard(Guid flashcardId, Guid userId);
    Task<IEnumerable<Flashcard>> GetOwnFlashcard(FlashcardQueryFilter queryFilter, Guid userId);
    Task<Flashcard> GetFlashcardBySlug(string slug, Guid? userId);
    Task<IEnumerable<Flashcard>> GetFlashcards();
    Task<List<string>> GetFlashcardBySubjectId(IEnumerable<string> subjectIds, CancellationToken cancellationToken = default);
    Task<List<Flashcard>> GetFlashcardForTips(IEnumerable<string> flashcardIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> ExceptExistFlashcards(IEnumerable<Guid> flashcardIds);
}