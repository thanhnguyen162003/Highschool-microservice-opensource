using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Helper;

namespace Infrastructure.Repositories
{
    public class TagRepository(DocumentDbContext context) : BaseRepository<Tag>(context), ITagRepository
    {
        private readonly DocumentDbContext _context = context;

        public async Task<List<Tag>> GetOrCreateTagsAsync(List<(string Name, string NormalizedName, Guid Id)> tagInfos, CancellationToken cancellationToken = default)
        {
            if (tagInfos == null || !tagInfos.Any())
                return new List<Tag>();

            // Lấy các normalized name
            var normalizedNames = tagInfos.Select(t => t.NormalizedName.ToLower()).ToList();

            // Lấy tags đã tồn tại
            var existingTags = await _context.Tags
                .Where(t => normalizedNames.Contains(t.NormalizedName.ToLower()))
                .ToListAsync(cancellationToken);

            // Tìm tags cần tạo mới
            var existingNormalizedNames = existingTags.Select(t => t.NormalizedName.ToLower()).ToHashSet();
            var tagsToCreate = tagInfos
                .Where(t => !existingNormalizedNames.Contains(t.NormalizedName.ToLower()))
                .Select(t => new Tag
                {
                    Id = t.Id,
                    Name = t.Name,
                    NormalizedName = t.NormalizedName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                })
                .ToList();

            if (tagsToCreate.Any())
            {
                await _context.Tags.AddRangeAsync(tagsToCreate, cancellationToken);
            }

            return existingTags.Concat(tagsToCreate).ToList();
        }

        public async Task<bool> AddTagsToFlashcardAsync(Guid flashcardId, List<Guid> tagIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingTags = await _context.FlashcardTags
                    .Where(ft => ft.FlashcardId == flashcardId)
                    .Select(ft => ft.TagId)
                    .ToListAsync(cancellationToken);

                var newTagIds = tagIds.Except(existingTags).ToList();

                if (!newTagIds.Any())
                    return true;

                var flashcardTags = newTagIds.Select(tagId => new FlashcardTag
                {
                    FlashcardId = flashcardId,
                    TagId = tagId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                await _context.FlashcardTags.AddRangeAsync(flashcardTags, cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Tag>> GetTagsByFlashcardIdAsync(Guid flashcardId, CancellationToken cancellationToken = default)
        {
            return await _context.FlashcardTags
                .Where(ft => ft.FlashcardId == flashcardId)
                .Select(ft => ft.Tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Tag>> SearchTagsAsync(string searchTerm, string normalizedSearchTerm, int limit = 10, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(searchTerm) && string.IsNullOrEmpty(normalizedSearchTerm))
                return await _context.Tags.Take(limit).ToListAsync(cancellationToken);

            return await _context.Tags
                .Where(t => (string.IsNullOrEmpty(normalizedSearchTerm) || t.NormalizedName.Contains(normalizedSearchTerm)) ||
                            (string.IsNullOrEmpty(searchTerm) || t.Name.Contains(searchTerm)))
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> RemoveTagsFromFlashcardAsync(Guid flashcardId, List<Guid>? specificTagIds = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.FlashcardTags.Where(ft => ft.FlashcardId == flashcardId);

                if (specificTagIds != null && specificTagIds.Any())
                {
                    query = query.Where(ft => specificTagIds.Contains(ft.TagId));
                }

                var tagsToRemove = await query.ToListAsync(cancellationToken);

                if (tagsToRemove.Any())
                {
                    _context.FlashcardTags.RemoveRange(tagsToRemove);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Tag>> GetTagsPaginated(string? searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Tags.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearch = StringHelper.NormalizeVietnamese(searchTerm.ToLower());
                query = query.Where(t =>
                    t.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    t.NormalizedName.ToLower().Contains(normalizedSearch)
                );
            }

            // Include count information
            return await query
                .OrderByDescending(t => t.FlashcardTags.Count)
                .ThenBy(t => t.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new Tag
                {
                    Id = t.Id,
                    Name = t.Name,
                    NormalizedName = t.NormalizedName,
                    FlashcardTags = t.FlashcardTags
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountTagsAsync(string? searchTerm, CancellationToken cancellationToken = default)
        {
            var query = _context.Tags.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearch = StringHelper.NormalizeVietnamese(searchTerm.ToLower());
                query = query.Where(t =>
                    t.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    t.NormalizedName.ToLower().Contains(normalizedSearch)
                );
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<List<Tag>> GetPopularTagsAsync(int limit = 10, CancellationToken cancellationToken = default)
        {
            return await _context.Tags
                .AsNoTracking()
                .OrderByDescending(t => t.FlashcardTags.Count)
                .Take(limit)
                .Select(t => new Tag
                {
                    Id = t.Id,
                    Name = t.Name,
                    NormalizedName = t.NormalizedName,
                    FlashcardTags = t.FlashcardTags
                })
                .ToListAsync(cancellationToken);
        }
    }
}