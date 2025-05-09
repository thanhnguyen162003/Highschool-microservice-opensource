using Domain.Enums;
using Infrastructure.Constraints;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
    public class FolderUserRepository(DocumentDbContext context) : BaseRepository<FolderUser>(context), IFolderUserRepository
    {
        public async Task<int> GetFoldersCount(CancellationToken cancellationToken = default)
        {
            return await _entities.CountAsync();
        }
        public async Task<IEnumerable<FolderUser>> GetFolderByUserId(Guid userId)
        {
            return await _entities
                .Include(e => e.FlashcardFolders).ThenInclude(e => e.Flashcard)
                .Include(e => e.DocumentFolders)
                .Where(e => e.UserId.Equals(userId) && e.Visibility!.Equals(VisibilityFolder.Public.ToString()))
                .ToListAsync();
        }

        public async Task<IEnumerable<FolderUser>> GetMyFolder(Guid userId)
        {
            return await _entities
                .Include(e => e.FlashcardFolders).ThenInclude(e => e.Flashcard)
                .Include(e => e.DocumentFolders)
                .Where(e => e.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<IEnumerable<FolderUser>> GetFolderUsers(Guid userId)
        {   
            return await _entities
                .Include(e => e.FlashcardFolders).ThenInclude(e => e.Flashcard)
                .Include(e => e.DocumentFolders)
                .Where(e => (e.UserId.Equals(userId) && e.Visibility!.Equals(VisibilityFolder.Private.ToString()) || 
                        e.Visibility!.Equals(VisibilityFolder.Public.ToString()))).ToListAsync();
        }

        public async Task<IEnumerable<FolderUser>> GetFolderUsers()
        {
            return await _entities
                .Include(e => e.FlashcardFolders).ThenInclude(e => e.Flashcard)
                .Include(e => e.DocumentFolders)
                .Where(e => e.Visibility!.Equals(VisibilityFolder.Public.ToString())).ToListAsync();
        }

        public async Task<IEnumerable<FolderUser>> GetFolders()
        {
            return await _entities
                .Include(e => e.FlashcardFolders).ThenInclude(e => e.Flashcard)
                .Include(e => e.DocumentFolders)
                .ToListAsync();
        }

        public async Task<FolderUser?> GetById(Guid folderId)
        {
            return await _entities.FirstOrDefaultAsync(f => f.Id.Equals(folderId) && f.Visibility!.Equals(VisibilityFolder.Public.ToString()));
        }

        public async Task<FolderUser?> GetById(Guid folderId, Guid userId)
        {
            return await _entities.FirstOrDefaultAsync(f => (f.Id.Equals(folderId) && f.UserId.Equals(userId)) || 
                                            (f.Id.Equals(folderId) && f.Visibility!.Equals(VisibilityFolder.Public.ToString())));
        }

    }
}
