using System.Linq.Expressions;
using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface IFlashcardContentRepository : IRepository<FlashcardContent>
{
    Task<List<FlashcardContent>> GetFlashcardContent(FlashcardQueryFilter queryFilter, Guid flashcardId);
	Task<List<FlashcardContent>> GetFlashcardContentByFlashcardId(Guid flashcardId);
	Task<List<FlashcardContent>> GetFlashcardContentByIds(List<Guid> flashcardContentIds);
	Task<FlashcardContent> GetFlashcardContentByRank(int rank);
	Task<FlashcardContent> GetFlashcardContentById(Guid flashcardContentId);
	Task<bool> UpdateRank(FlashcardContent flashcardContent);
	Task<IEnumerable<FlashcardContent>> GetAllAsync(Expression<Func<FlashcardContent, bool>> predicate);
	Task<List<FlashcardContent>> GetFlashcardContentByIds(IEnumerable<Guid?> listId);
    Task<bool> CreateFlashcardContent(List<FlashcardContent> flashcardContents); 
	Task<bool> CreateFlashcardContentSingle(FlashcardContent flashcardContent);
    Task<bool> UpdateFlashcardContent(FlashcardContent flashcard, Guid id);
    Task<bool> UpdateListFlashcardContent(List<FlashcardContent?> flashcard);
    Task<bool> DeleteFlashcardContent(Guid flashcardContentId, Guid userId);
}