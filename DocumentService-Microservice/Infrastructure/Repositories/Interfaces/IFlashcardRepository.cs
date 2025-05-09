using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface IFlashcardRepository : IRepository<Flashcard>
{
    Task<IEnumerable<Flashcard>> GetFlashcards(FlashcardQueryFilter queryFilter, CancellationToken cancellationToken = default);
    Task<IEnumerable<Flashcard>> GetFlashcardsManagement(FlashcardQueryFilterManagement queryFilter, CancellationToken cancellationToken = default);
    Task<IEnumerable<Flashcard>> GetFlashcardsPlaceholder();
    Task<IEnumerable<Flashcard>> GetTopFlashcard();
	Task<IEnumerable<Flashcard>> EveryOneAlsoWatch();
    Task<IEnumerable<Flashcard>> GetFlashcardsWithToken(FlashcardQueryFilter queryFilter, Guid userId, CancellationToken cancellationToken = default);
    Task<Flashcard> GetFlashcardById(Guid flashcardId);
    Task<Flashcard> GetFlashcardByIdNoStatus(Guid flashcardId);
    Task<Flashcard> GetFlashcardByIdWithToken(Guid flashcardId, Guid? userId);
    Task<List<Flashcard>> GetFlashcardBySubjectIdFilter(List<Guid> subjectIds);
    Task<IEnumerable<Flashcard>> GetFlashcardsByUserId(FlashcardQueryFilter queryFilter, Guid userId, CancellationToken cancellationToken = default);
    Task<int> CheckNumberFlashcardInUser(Guid userId);
    Task<IEnumerable<Flashcard>> GetFlashcardByUsername(FlashcardQueryFilter queryFilter, string username);
    Task<Flashcard> GetFlashcardDraftById(Guid flashcardId, Guid userId);
    Task<Flashcard> GetFlashcardByUserId(Guid userId);
    Task<IEnumerable<Flashcard>> GetFlashcardsBySubject(FlashcardQueryFilter queryFilter, Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Flashcard>> GetFlashcardsBySubjectId(Guid? subjectId,
	    CancellationToken cancellationToken = default);
    Task<IEnumerable<Flashcard>> GetFlashcardsByLessonId(Guid? lessonId,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Flashcard>> GetFlashcardsByChapterId(Guid? chapterId,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Flashcard>> GetFlashcardsBySubjectCurriculumId(Guid? subjectCurriculumId,
        CancellationToken cancellationToken = default);
    Task<Flashcard> GetFlashcardDraftByUserId(Guid userId);
    Task<Flashcard> GetFlashcardDraftAIByUserId(Guid userId);
    Task<bool> CreateFlashcard(Flashcard flashcard);
    Task<bool> UpdateFlashcard(Flashcard flashcard, Guid userId);
    Task<bool> UpdateCreatedFlashcard(Flashcard flashcard, Guid id);
    Task<bool> DeleteFlashcard(Guid flashcardId, Guid userId);
    Task<IEnumerable<Flashcard>> GetOwnFlashcard(FlashcardQueryFilter queryFilter, Guid userId, CancellationToken cancellationToken = default);
    Task<Flashcard> GetFlashcardBySlug(string slug, Guid? userId);
    Task<IEnumerable<Flashcard>> GetFlashcards();
    Task<List<string>> GetFlashcardBySubjectId(IEnumerable<string> subjectIds, CancellationToken cancellationToken = default);
    Task<List<Flashcard>> GetFlashcardForTips(IEnumerable<string> flashcardIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> ExceptExistFlashcards(IEnumerable<Guid> flashcardIds);
    Task<IEnumerable<Flashcard>> SearchFlashcardsFullText(string searchTerm, int pageNumber, int pageSize, List<string>? tags = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Flashcard>> SearchFlashcardsFullTextManagement(
        string searchTerm,
        int pageNumber,
        int pageSize,
        Guid? userId,
        bool? isCreatedBySystem,
        string? status,
        bool? isDeleted,
        List<string>? tags = null,
        CancellationToken cancellationToken = default);
    Task<int> GetTotalFlashcard();
    Task<int> GetTotalFlashcardDraft();
    Task<int> GetTotalFlashcardOpen();
    Task<int> GetTotalFlashcardLink();
    Task<int> GetTotalThisMonthFlashcard();
    Task<int> GetTotalLastMonthFlashcard();
    Task<Dictionary<DateTime, int>> GetFlashcardsCountByDay(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<Dictionary<DateTime, int>> GetFlashcardsCountByTime(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    //recommended
    Task<List<Flashcard>> GetFlashcardsByMasterSubjectId(Guid masterSubjectId, int? grade, CancellationToken cancellationToken);
    Task<List<Flashcard>> GetFlashcardsPlaceholder(int? grade = null);
    Task<List<Flashcard>> GetFlashcardsPlaceholderNoGrade(int numberMissing);
}