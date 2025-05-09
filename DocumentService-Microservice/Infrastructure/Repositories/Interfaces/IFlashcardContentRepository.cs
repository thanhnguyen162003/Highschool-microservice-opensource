using System.Linq.Expressions;
using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface IFlashcardContentRepository : IRepository<FlashcardContent>
{
    Task<List<FlashcardContent>> GetFlashcardContent(FlashcardQueryFilter queryFilter, Guid flashcardId);
	Task<List<FlashcardContent>> GetFlashcardContentByFlashcardId(Guid flashcardId);
	Task<List<FlashcardContent>> GetFlashcardContentByIds(List<Guid> flashcardContentIds);
	Task<FlashcardContent> GetFlashcardContentByRank(int rank, CancellationToken cancellationToken);
	Task<FlashcardContent> GetFlashcardContentById(Guid flashcardContentId);
	void UpdateRank(FlashcardContent flashcardContent);
	Task<IEnumerable<FlashcardContent>> GetAllAsync(Expression<Func<FlashcardContent, bool>> predicate);
	Task<List<FlashcardContent>> GetFlashcardContentByIds(IEnumerable<Guid?> listId);
    Task<bool> CreateFlashcardContent(List<FlashcardContent> flashcardContents); 
	Task<bool> CreateFlashcardContentSingle(FlashcardContent flashcardContent);
    Task<bool> UpdateFlashcardContent(FlashcardContent flashcard, Guid id);
    Task<bool> UpdateListFlashcardContent(List<FlashcardContent?> flashcard);
    Task<bool> DeleteFlashcardContent(Guid flashcardContentId, Guid userId);
	Task<List<FlashcardContent>> GetFlashcardsWithinRankRange(int minRank, int maxRank, CancellationToken cancellationToken);
    Task<List<FlashcardContent>> GetFlashcardContentSort(Guid flashcardId, Guid userId);
	Task<List<FlashcardContent>> GetFlashcardContentStarred(FlashcardQueryFilter queryFilter, Guid flashcardId, Guid userId);
}