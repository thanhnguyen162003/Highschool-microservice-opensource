using Infrastructure.Constraints;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Repositories
{
    public class FlashcardFolderRepository(DocumentDbContext context) : BaseRepository<FlashcardFolder>(context), IFlashcardFolderRepository
    {
        public async Task<IEnumerable<FlashcardFolder>> GetFlashcardFolderByFolderId(Guid folderId, Guid? userId)
        {
            if(userId.HasValue || !userId.Equals(Guid.Empty)) 
                return await _entities
                .Include(e => e.Folder)
                .Include(e => e.Flashcard)
                .Include(f => f.Flashcard!.FlashcardContents)
                .AsSplitQuery()
                .Where(e => e.FolderId.Equals(folderId) &&
                            (e.Flashcard!.UserId.Equals(userId) ||
                             (!e.Flashcard.UserId.Equals(userId) && e.Flashcard!.Status.Equals(StatusConstrains.OPEN))))
                .ToListAsync();

            return await _entities
                .Include(e => e.Folder)
                .Include(e => e.Flashcard)
                .Include(f => f.Flashcard!.FlashcardContents)
                .AsSplitQuery()
                .Where(e => e.FolderId.Equals(folderId) && e.Flashcard!.Status.Equals(StatusConstrains.OPEN))
                .ToListAsync();
        }

        public async Task DeleteFlashcardOnFolder(Guid folderId)
        {
            var flashcards = await _entities.Where(e => e.FolderId.Equals(folderId)).ToListAsync();

            _entities.RemoveRange(flashcards);

        }

        public async Task AddFlashcardOnFolder(IEnumerable<Guid> flashcardIds, Guid folderId)
        {
            foreach (var flashcardId in flashcardIds)
            {
                var flashcardFolder = new FlashcardFolder()
                {
                    Id = Guid.NewGuid(),
                    FolderId = folderId,
                    FlashcardId = flashcardId,
                    CreatedAt = DateTime.UtcNow
                };

                await _entities.AddAsync(flashcardFolder);
            }
        }

        public async Task<Guid?> DeleteFlashcard(Guid flashcardId, Guid folderId)
        {
            var flashcardOnFolder = await _entities.FirstOrDefaultAsync(e => e.FlashcardId.Equals(flashcardId) && e.FolderId.Equals(folderId));
            
            if(flashcardOnFolder != null)
            {
                _entities.Remove(flashcardOnFolder);

                return flashcardOnFolder.FolderId;
            }

            return null;
        }

        public async Task<IEnumerable<Guid>> NotExistOnFolder(IEnumerable<Guid> flashcardIds, Guid folderId)
        {
            var flashcards = await _entities
                .AsNoTracking()
                .Where(e => flashcardIds.Contains(e.FlashcardId) && e.FolderId.Equals(folderId))
                .Select(e => e.FlashcardId)
                .ToListAsync();

            return flashcardIds.Except(flashcards);

        }

    }
}
