using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IFlashcardFolderRepository : IRepository<FlashcardFolder>
    {
        Task<IEnumerable<FlashcardFolder>> GetFlashcardFolderByFolderId(Guid folderId, Guid? userId);
        Task DeleteFlashcardOnFolder(Guid folderId);
        Task AddFlashcardOnFolder(IEnumerable<Guid> flashcardIds, Guid folderId);
        Task<Guid?> DeleteFlashcard(Guid flashcardId, Guid folderId);
        Task<IEnumerable<Guid>> NotExistOnFolder(IEnumerable<Guid> flashcardIds, Guid folderId);
    }
}
