using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<List<Tag>> GetOrCreateTagsAsync(List<(string Name, string NormalizedName, Guid Id)> tagInfos, CancellationToken cancellationToken = default);
        Task<bool> AddTagsToFlashcardAsync(Guid flashcardId, List<Guid> tagIds, CancellationToken cancellationToken = default);
        Task<List<Tag>> GetTagsByFlashcardIdAsync(Guid flashcardId, CancellationToken cancellationToken = default);
        Task<List<Tag>> SearchTagsAsync(string searchTerm, string normalizedSearchTerm, int limit = 10, CancellationToken cancellationToken = default);
        Task<bool> RemoveTagsFromFlashcardAsync(Guid flashcardId, List<Guid>? specificTagIds = null, CancellationToken cancellationToken = default);
        Task<List<Tag>> GetTagsPaginated(string? searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<int> CountTagsAsync(string? searchTerm, CancellationToken cancellationToken = default);
        Task<List<Tag>> GetPopularTagsAsync(int limit = 10, CancellationToken cancellationToken = default);
    }
}
