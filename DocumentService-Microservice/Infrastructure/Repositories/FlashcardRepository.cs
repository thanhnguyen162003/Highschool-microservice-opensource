using Domain.Enums;
using Domain.QueriesFilter;
using Infrastructure.Constraints;
using Infrastructure.Contexts;
using Infrastructure.Helper;
using Infrastructure.Repositories.Interfaces;
using MediatR;
using System.ComponentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories;

public class FlashcardRepository(DocumentDbContext context) : BaseRepository<Flashcard>(context), IFlashcardRepository
{
    private readonly DocumentDbContext _context = context;

    public async Task<Dictionary<DateTime, int>> GetFlashcardsCountByDay(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var query = _entities
            .AsNoTracking()
            .Where(x => x.DeletedAt == null && x.CreatedAt >= start && x.CreatedAt <= end);

        var result = await query
            .GroupBy(x => new DateTime(x.CreatedAt.Year, x.CreatedAt.Month, x.CreatedAt.Day, 0, 0, 0, DateTimeKind.Utc))
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count, cancellationToken);

        return result;
    }

    public async Task<Dictionary<DateTime, int>> GetFlashcardsCountByTime(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var query = _entities
            .AsNoTracking()
            .Where(x => x.DeletedAt == null && x.CreatedAt >= start && x.CreatedAt <= end);

        var result = await query
            .GroupBy(x => new DateTime(x.CreatedAt.Year, x.CreatedAt.Month, x.CreatedAt.Day, x.CreatedAt.Hour, 0, 0, DateTimeKind.Utc))
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count, cancellationToken);

        return result;
    }

    private async Task LoadRelatedEntities(Flashcard flashcard, CancellationToken cancellationToken = default)
    {
        if (flashcard.SubjectId != Guid.Empty)
        {
            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId, cancellationToken);
            if (subjectExists)
            {
                await _context.Entry(flashcard)
                    .Reference(f => f.Subject)
                    .Query()
                    .LoadAsync(cancellationToken);
            }
        }

        if (flashcard.SubjectCurriculumId != Guid.Empty)
        {
            var curriculumExists = await _context.SubjectCurricula.AnyAsync(s => s.Id == flashcard.SubjectCurriculumId, cancellationToken);
            if (curriculumExists)
            {
                await _context.Entry(flashcard)
                    .Reference(f => f.SubjectCurriculum)
                    .Query()
                    .LoadAsync(cancellationToken);
            }
        }

        if (flashcard.ChapterId != Guid.Empty)
        {
            var chapterExists = await _context.Chapters.AnyAsync(s => s.Id == flashcard.ChapterId, cancellationToken);
            if (chapterExists)
            {
                await _context.Entry(flashcard)
                    .Reference(f => f.Chapter)
                    .Query()
                    .LoadAsync(cancellationToken);
            }
        }

        if (flashcard.LessonId != Guid.Empty)
        {
            var lessonExists = await _context.Lessons.AnyAsync(s => s.Id == flashcard.LessonId, cancellationToken);
            if (lessonExists)
            {
                await _context.Entry(flashcard)
                    .Reference(f => f.Lesson)
                    .Query()
                    .LoadAsync(cancellationToken);
            }
        }
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcards()
    {
        return await _entities
            .AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.DeletedAt == null && x.Created == true)
            .Include(y => y.FlashcardContents)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Lesson)
                .ThenInclude(l => l.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .ToListAsync();
    }

    public async Task<int> GetTotalFlashcard()
    {
        return await _entities.CountAsync(x => x.DeletedAt == null);
    }
    public async Task<int> GetTotalThisMonthFlashcard()
    {
        return await _entities
            .CountAsync(x => x.CreatedAt >= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0,DateTimeKind.Utc) && x.DeletedAt == null);
    }
    public async Task<int> GetTotalLastMonthFlashcard()
    {
        var test = DateTime.DaysInMonth(DateTime.UtcNow.AddMonths(-1).Year, DateTime.UtcNow.AddMonths(-1).Month);
        return await _entities.CountAsync(x => x.CreatedAt >= new DateTime(DateTime.UtcNow.AddMonths(-1).Year, DateTime.UtcNow.AddMonths(-1).Month, 1, 0, 0, 0, DateTimeKind.Utc) && x.CreatedAt <= new DateTime(DateTime.UtcNow.AddMonths(-1).Year, DateTime.UtcNow.AddMonths(-1).Month, test, 23, 59, 59,DateTimeKind.Utc));
    }

    public async Task<int> GetTotalFlashcardDraft()
    {
        return await _entities
            .Where(x => x.Created == false && x.DeletedAt == null)
            .CountAsync();
    }

    public async Task<int> GetTotalFlashcardOpen()
    {
        return await _entities
            .Where(x => x.Status == "Open" && x.DeletedAt == null)
            .CountAsync();
    }
    public async Task<int> GetTotalFlashcardLink()
    {
        return await _entities
            .Where(x => x.Status == "Link" && x.DeletedAt == null)
            .CountAsync();
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcards(FlashcardQueryFilter queryFilter, CancellationToken cancellationToken = default)
    {
        var flashcards = _entities.AsNoTracking()
            .Include(f => f.FlashcardContents)
            .Include(f => f.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Lesson)
                .ThenInclude(l => l.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true)
            .AsQueryable();

        flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);

        if (queryFilter.Tags != null && queryFilter.Tags.Any())
        {
            var lowerTags = queryFilter.Tags.Select(t => t.ToLower()).ToList();
            flashcards = flashcards.Where(f =>
                lowerTags.All(tag =>
                    f.FlashcardTags.Any(ft =>
                        ft.Tag.Name.ToLower() == tag ||
                        ft.Tag.NormalizedName.ToLower() == tag)));
        }

        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        return await flashcards.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsManagement(FlashcardQueryFilterManagement queryFilter, CancellationToken cancellationToken = default)
    {
        var flashcards = _entities.AsNoTracking()
            .Include(f => f.FlashcardContents)
            .Include(f => f.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
            .Include(f => f.Chapter)
            .Include(f => f.Lesson)
            .Where(f => f.Created)
            .AsQueryable();

        if (!string.IsNullOrEmpty(queryFilter.Status) && (queryFilter.Status.Equals(StatusConstrains.OPEN)
                                                        || queryFilter.Status.Equals(StatusConstrains.CLOSE)
                                                        || queryFilter.Status.Equals(StatusConstrains.HIDDEN)
                                                        || queryFilter.Status.Equals(StatusConstrains.ONLYLINK)))
        {
            flashcards = flashcards.Where(x => x.Status.Equals(queryFilter.Status));
        }

        if (queryFilter.IsDeleted.HasValue)
        {
            flashcards = queryFilter.IsDeleted.Value ? flashcards.Where(x => x.DeletedAt != null) : flashcards.Where(x => x.DeletedAt == null);
        }

        flashcards = ApplyFilterSortAndSearchManagement(flashcards, queryFilter);

        if (queryFilter.Tags != null && queryFilter.Tags.Any())
        {
            var lowerTags = queryFilter.Tags.Select(t => t.ToLower()).ToList();
            flashcards = flashcards.Where(f =>
                lowerTags.All(tag =>
                    f.FlashcardTags.Any(ft =>
                        ft.Tag.Name.ToLower() == tag ||
                        ft.Tag.NormalizedName.ToLower() == tag)));
        }

        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        return await flashcards.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsPlaceholder()
    {
        return await _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true && x.DeletedAt == null)
            .OrderBy(x => x.CreatedAt)
            .Take(8)
            .Include(y => y.FlashcardContents)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
            .Include(f => f.Chapter)
            .Include(f => f.Lesson)
            .ToListAsync();
    }

    public async Task<IEnumerable<Flashcard>> GetTopFlashcard()
    {
        return await _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true && x.DeletedAt == null)
            .OrderBy(x => x.Vote)
            .ThenBy(x => x.TodayView)
            .ThenBy(x => x.TotalView)
            .Take(8)
            .Include(y => y.FlashcardContents)
            .Include(y => y.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
            .Include(f => f.Chapter)
            .Include(f => f.Lesson)
            .ToListAsync();
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsWithToken(FlashcardQueryFilter queryFilter, Guid userId, CancellationToken cancellationToken = default)
    {
        var flashcards = _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) || x.UserId.Equals(userId) && x.Created == true)
            .Include(y => y.FlashcardContents)
            .Include(f => f.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Lesson)
                .ThenInclude(l => l.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .AsQueryable();

        flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);

        if (queryFilter.Tags != null && queryFilter.Tags.Any())
        {
            var lowerTags = queryFilter.Tags.Select(t => t.ToLower()).ToList();
            flashcards = flashcards.Where(f =>
                lowerTags.All(tag =>
                    f.FlashcardTags.Any(ft =>
                        ft.Tag.Name.ToLower() == tag ||
                        ft.Tag.NormalizedName.ToLower() == tag)));
        }

        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        return await flashcards.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Flashcard>> GetOwnFlashcard(FlashcardQueryFilter queryFilter, Guid userId, CancellationToken cancellationToken = default)
    {
        var flashcards = _entities.AsNoTracking()
            .Where(x => x.UserId.Equals(userId) && x.DeletedAt == null)
            .Include(y => y.FlashcardContents)
            .Include(f => f.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Lesson)
                .ThenInclude(l => l.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .AsQueryable();

        flashcards = ApplyFilterSortAndSearchOwn(flashcards, queryFilter);

        if (queryFilter.Tags != null && queryFilter.Tags.Any())
        {
            var lowerTags = queryFilter.Tags.Select(t => t.ToLower()).ToList();
            foreach (var tag in lowerTags)
            {
                string currentTag = tag;
                flashcards = flashcards.Where(f =>
                    f.FlashcardTags.Any(ft =>
                        ft.Tag.Name.ToLower() == currentTag ||
                        ft.Tag.NormalizedName.ToLower() == currentTag
                    )
                );
            }
        }

        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        return await flashcards.ToListAsync(cancellationToken);
    }

    public async Task<Flashcard> GetFlashcardById(Guid flashcardId)
    {
        return await _entities
            .AsNoTracking()
            .Where(x =>
                x.Id.Equals(flashcardId) &&
                x.DeletedAt == null &&
                x.Created == true
            )
            .Include(y => y.FlashcardContents)
            .Include(y => y.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Lesson)
                .ThenInclude(l => l.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .FirstOrDefaultAsync();
    }
    public async Task<Flashcard> GetFlashcardByIdNoStatus(Guid flashcardId)
    {
        return (await _entities.AsNoTracking().Where(x => x.Id.Equals(flashcardId) && x.DeletedAt == null)
            .Include(y => y.FlashcardContents)
            .FirstOrDefaultAsync())!;
    }

    public async Task<Flashcard> GetFlashcardByIdWithToken(Guid flashcardId, Guid? userId)
    {
        return await _entities
            .AsNoTracking()
            .Where(x =>
                x.Id.Equals(flashcardId) &&
                x.DeletedAt == null &&
                (
                    x.Created == true ||
                    (userId != Guid.Empty && x.Created == false && x.UserId == userId)
                )
            )
            .Include(y => y.FlashcardContents)
            .Include(y => y.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Lesson)
                .ThenInclude(l => l.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Flashcard>> GetFlashcardBySubjectIdFilter(List<Guid> subjectIds)
    {
        // Query to get flashcards where SubjectId is in the list of provided subjectIds
        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => subjectIds.Contains((Guid)x.SubjectId) && x.DeletedAt == null && x.Status.Equals(StatusConstrains.OPEN) && x.Created == true) 
            .Include(y => y.FlashcardContents)
            .Take(8)
            .ToListAsync(); 
        return flashcards;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsByUserId(FlashcardQueryFilter queryFilter, Guid userId, CancellationToken cancellationToken = default)
    {
        var flashcards = _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.UserId.Equals(userId) && x.Created == true)
            .Include(y => y.FlashcardContents)
            .Include(f => f.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(s => s.Subject!)
            .AsQueryable();

        flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);

        if (queryFilter.Tags != null && queryFilter.Tags.Any())
        {
            var lowerTags = queryFilter.Tags.Select(t => t.ToLower()).ToList();
            foreach (var tag in lowerTags)
            {
                string currentTag = tag;
                flashcards = flashcards.Where(f =>
                    f.FlashcardTags.Any(ft =>
                        ft.Tag.Name.ToLower() == currentTag ||
                        ft.Tag.NormalizedName.ToLower() == currentTag
                    )
                );
            }
        }

        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        return await flashcards.ToListAsync(cancellationToken);
    }

    public Task<int> CheckNumberFlashcardInUser(Guid userId)
    {
        var flashcardsNumber = _entities.AsNoTracking().Where(x => x.UserId.Equals(userId)).CountAsync();
        return flashcardsNumber;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardByUsername(FlashcardQueryFilter queryFilter, string username)
    {
        throw new NotImplementedException();
    }

    public async Task<Flashcard> GetFlashcardDraftById(Guid flashcardId, Guid userId)
    {
        return (await _entities.AsNoTracking()
            .Where(x => x.Id.Equals(flashcardId) && x.Created == false && x.UserId.Equals(userId))
            .Include(y => y.FlashcardContents)
            .Include(y => y.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .FirstOrDefaultAsync())!;
    }

    public async Task<Flashcard> GetFlashcardByUserId(Guid userId)
    {
        return await _entities.AsNoTracking().Where(x => x.UserId.Equals(userId) && x.DeletedAt == null && x.Created == true).FirstOrDefaultAsync();
    }
    public async Task<Flashcard> GetFlashcardDraftByUserId(Guid userId)
    {
        return (await _entities.AsNoTracking().Where(x => x.UserId.Equals(userId) && x.Created == false && x.DeletedAt == null && x.IsArtificalIntelligence == false).FirstOrDefaultAsync())!;
    }

    public async Task<Flashcard> GetFlashcardDraftAIByUserId(Guid userId)
    {
        return (await _entities.AsNoTracking().Where(x => x.UserId.Equals(userId) && x.Created == false && x.DeletedAt == null && x.IsArtificalIntelligence == true).FirstOrDefaultAsync())!;
    }

    public async Task<Flashcard> GetFlashcardBySlug(string slug, Guid? userId)
    {
        return await _entities.AsNoTracking()
            .Where(x =>
                x.Slug.Equals(slug) &&
                (x.Status.Equals(StatusConstrains.OPEN) ||
                x.Status.Equals(StatusConstrains.ONLYLINK) ||
                (userId != Guid.Empty && x.UserId == userId)) &&
                x.DeletedAt == null
            )
            .Include(f => f.FlashcardContents)
            .Include(f => f.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Lesson)
                .ThenInclude(l => l.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteFlashcard(Guid flashcardId, Guid userId)
    {
        var flashcards = await _entities.Where(x => x.Id.Equals(flashcardId) && x.UserId.Equals(userId) && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (flashcards is null)
        {
            return false;
        }
        flashcards.DeletedAt = DateTime.UtcNow;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<Flashcard> GetFlashcardDetail(Guid flashcardId)
    {
        return (await _entities.AsNoTracking()
            .Include(y=> y.FlashcardContents)
            .FirstOrDefaultAsync())!;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsBySubject(FlashcardQueryFilter queryFilter, Guid id, CancellationToken cancellationToken = default)
    {
        var flashcards = _entities
            .Where(x => x.SubjectId.Equals(id) && (x.Status.Equals(StatusConstrains.OPEN)))
            .Include(y => y.FlashcardContents)
            .Include(f => f.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .AsQueryable();

        flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);

        if (queryFilter.Tags != null && queryFilter.Tags.Any())
        {
            var lowerTags = queryFilter.Tags.Select(t => t.ToLower()).ToList();
            foreach (var tag in lowerTags)
            {
                string currentTag = tag;
                flashcards = flashcards.Where(f =>
                    f.FlashcardTags.Any(ft =>
                        ft.Tag.Name.ToLower() == currentTag ||
                        ft.Tag.NormalizedName.ToLower() == currentTag
                    )
                );
            }
        }

        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        return await flashcards.ToListAsync(cancellationToken);
    }

    public async Task<bool> CreateFlashcard(Flashcard flashcard)
    {
        _context.Flashcards.Add(flashcard);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateFlashcard(Flashcard flashcard, Guid id)
    {
        var flashcardUpdate = await _entities.Where(x => x.Id.Equals(flashcard.Id) && x.UserId.Equals(id) && x.DeletedAt == null).FirstOrDefaultAsync();

        if (flashcardUpdate == null)
        {
            return false;
        }
        flashcardUpdate.FlashcardName = flashcard.FlashcardName ?? flashcardUpdate.FlashcardName;
        flashcardUpdate.SubjectId = flashcard.SubjectId;
        flashcardUpdate.SubjectCurriculumId = flashcard.SubjectCurriculumId;
        flashcardUpdate.ChapterId = flashcard.ChapterId;
        flashcardUpdate.LessonId = flashcard.LessonId;
        flashcardUpdate.FlashcardType = flashcard.FlashcardType;
        flashcardUpdate.FlashcardDescription = flashcard.FlashcardDescription ?? flashcardUpdate.FlashcardDescription;
        flashcardUpdate.Slug = flashcard.Slug ?? flashcardUpdate.Slug;
        flashcardUpdate.Status = flashcard.Status ?? flashcardUpdate.Status;
        flashcardUpdate.UpdatedAt = DateTime.UtcNow;

        _entities.Update(flashcardUpdate);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }
    public async Task<bool> UpdateCreatedFlashcard(Flashcard flashcard, Guid id)
    {
        _entities.Update(flashcard);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    private IQueryable<Flashcard> ApplyFilterSortAndSearch(IQueryable<Flashcard> flashcards, FlashcardQueryFilter queryFilter)
    {
        flashcards = flashcards.Where(x => x.DeletedAt == null);

        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            var searchTerm = queryFilter.Search.ToLower();

            flashcards = flashcards.Where(x =>
                x.FlashcardName.ToLower().Contains(searchTerm) ||
                (x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(searchTerm)) ||
                x.FlashcardTags.Any(ft =>
                    ft.Tag.Name.ToLower().Contains(searchTerm) ||
                    ft.Tag.NormalizedName.ToLower().Contains(searchTerm))
            );
        }

        if (queryFilter.FlashcardType.HasValue)
        {
            if (queryFilter.EntityId.HasValue && queryFilter.EntityId != Guid.Empty)
            {
                var entityId = queryFilter.EntityId.Value;
                switch (queryFilter.FlashcardType.Value)
                {
                    case FlashcardType.Lesson:
                        flashcards = flashcards.Where(f => f.LessonId == entityId);
                        break;
                    case FlashcardType.Chapter:
                        flashcards = flashcards.Where(f => f.ChapterId == entityId
                                                    || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.ChapterId == entityId));
                        break;
                    case FlashcardType.SubjectCurriculum:
                        flashcards = flashcards.Where(f => f.SubjectCurriculumId == entityId
                                                    || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.Chapter.SubjectCurriculumId == entityId)
                                                    || (f.ChapterId != null && f.ChapterId != Guid.Empty && f.Chapter!.SubjectCurriculumId == entityId));
                        break;
                    case FlashcardType.Subject:
                        flashcards = flashcards.Where(f => f.SubjectId == entityId
                                                    || (f.LessonId != null && f.LessonId != Guid.Empty && f.Lesson!.Chapter.SubjectCurriculum.SubjectId == entityId)
                                                    || (f.ChapterId != null && f.ChapterId != Guid.Empty && f.Chapter!.SubjectCurriculum.SubjectId == entityId)
                                                    || (f.SubjectCurriculumId != null && f.SubjectCurriculumId != Guid.Empty && f.SubjectCurriculum!.SubjectId == entityId));
                        break;
                }
            }
        }

        return flashcards;
    }

    private IQueryable<Flashcard> ApplyFilterSortAndSearchManagement(IQueryable<Flashcard> flashcards, FlashcardQueryFilterManagement queryFilter)
    {
        flashcards = flashcards.Where(x => x.DeletedAt == null);

        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            var searchTerm = queryFilter.Search.ToLower();

            flashcards = flashcards.Where(x =>
                x.FlashcardName.ToLower().Contains(searchTerm) ||
                (x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(searchTerm)) ||
                x.FlashcardTags.Any(ft =>
                    ft.Tag.Name.ToLower().Contains(searchTerm) ||
                    ft.Tag.NormalizedName.ToLower().Contains(searchTerm))
            );
        }

        if (queryFilter.UserId != Guid.Empty && queryFilter.UserId != null)
        {
            flashcards = flashcards.Where(x => x.UserId == queryFilter.UserId);
        }

        if (queryFilter.IsCreatedBySystem.HasValue)
        {
            flashcards = flashcards.Where(x => x.IsCreatedBySystem == queryFilter.IsCreatedBySystem);
        }

        if (queryFilter.FlashcardType.HasValue)
        {
            flashcards = flashcards.Where(x => x.FlashcardType == queryFilter.FlashcardType);
            if (queryFilter.EntityId.HasValue && queryFilter.EntityId != Guid.Empty)
            {
                switch (queryFilter.FlashcardType.Value)
                {
                    case FlashcardType.Subject:
                        flashcards = flashcards.Where(f => f.SubjectId == queryFilter.EntityId.Value);
                        break;
                    case FlashcardType.SubjectCurriculum:
                        flashcards = flashcards.Where(f => f.SubjectCurriculumId == queryFilter.EntityId.Value);
                        break;
                    case FlashcardType.Chapter:
                        flashcards = flashcards.Where(f => f.ChapterId == queryFilter.EntityId.Value);
                        break;
                    case FlashcardType.Lesson:
                        flashcards = flashcards.Where(f => f.LessonId == queryFilter.EntityId.Value);
                        break;
                }
            }
        }

        return flashcards;
    }
    private IQueryable<Flashcard> ApplyFilterSortAndSearchOwn(IQueryable<Flashcard> flashcards, FlashcardQueryFilter queryFilter)
    {
        flashcards = flashcards.Where(x => x.DeletedAt == null);

        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            var searchTerm = queryFilter.Search.ToLower();

            flashcards = flashcards.Where(x =>
                x.FlashcardName.ToLower().Contains(searchTerm) ||
                (x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(searchTerm)) ||
                x.FlashcardTags.Any(ft =>
                    ft.Tag.Name.ToLower().Contains(searchTerm) ||
                    ft.Tag.NormalizedName.ToLower().Contains(searchTerm))
            );
        }

        if (queryFilter.FlashcardType.HasValue)
        {
            flashcards = flashcards.Where(x => x.FlashcardType == queryFilter.FlashcardType);
            if (queryFilter.EntityId.HasValue && queryFilter.EntityId != Guid.Empty)
            {
                switch (queryFilter.FlashcardType.Value)
                {
                    case FlashcardType.Subject:
                        flashcards = flashcards.Where(f => f.SubjectId == queryFilter.EntityId.Value);
                        break;
                    case FlashcardType.SubjectCurriculum:
                        flashcards = flashcards.Where(f => f.SubjectCurriculumId == queryFilter.EntityId.Value);
                        break;
                    case FlashcardType.Chapter:
                        flashcards = flashcards.Where(f => f.ChapterId == queryFilter.EntityId.Value);
                        break;
                    case FlashcardType.Lesson:
                        flashcards = flashcards.Where(f => f.LessonId == queryFilter.EntityId.Value);
                        break;
                }
            }
        }

        return flashcards;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsBySubjectId(Guid? subjectId,
        CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking()
            .Where(x => x.SubjectId.Equals(subjectId)&& x.Created == true)
            .Include(s=>s.Subject!)
            .OrderByDescending(x => x.CreatedAt)
            .Take(4)
            .ToListAsync(cancellationToken);
    }
    public async Task<List<string>> GetFlashcardBySubjectId(IEnumerable<string> subjectIds, CancellationToken cancellationToken = default)
    {
        if (subjectIds == null || !subjectIds.Any())
        {
            return null;
        }
        List<Guid> guids = subjectIds.Select(guid => Guid.Parse(guid)).ToList();
        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => guids.Contains((Guid)x.SubjectId))
            .Select(x => x.Id.ToString())
            .ToListAsync(cancellationToken);
        return flashcards;
    }
    public async Task<List<Flashcard>> GetFlashcardForTips(IEnumerable<string> flashcardIds, CancellationToken cancellationToken = default)
    {
        if (flashcardIds == null || !flashcardIds.Any())
        {
            return new List<Flashcard>();
        }

        List<Guid> guids = flashcardIds.Select(guid => Guid.Parse(guid)).ToList();

        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => guids.Contains(x.Id)) 
            .Select(x => new Flashcard { Id = x.Id, FlashcardName = x.FlashcardName, Slug = x.Slug })
            .ToListAsync(cancellationToken);

        return flashcards;
    }
    public async Task<IEnumerable<Guid>> ExceptExistFlashcards(IEnumerable<Guid> flashcardIds)
    {
        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => flashcardIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var result = flashcardIds.Except(flashcards).ToList();

        return result;
    }

    public async Task<IEnumerable<Flashcard>> EveryOneAlsoWatch()
    {
        return await _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true && x.DeletedAt == null)
            .OrderBy(x => x.TodayView)
            .ThenBy(x => x.TotalView)
            .ThenBy(x => x.Vote)
            .Take(20)
            .Include(y => y.FlashcardContents)
            .Include(y => y.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .Include(f => f.Lesson)
                .ThenInclude(l => l.Chapter)
                .ThenInclude(c => c.SubjectCurriculum)
                .ThenInclude(sc => sc.Subject)
            .ToListAsync();
    }

    public async Task<IEnumerable<Flashcard>> SearchFlashcardsFullText(
    string searchTerm,
    int pageNumber,
    int pageSize,
    List<string>? tags = null,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(searchTerm) && (tags == null || !tags.Any()))
            return await GetFlashcardsPlaceholder();

        // Chuẩn bị từ khóa tìm kiếm
        var originalSearchTerm = searchTerm?.ToLower()?.Trim() ?? string.Empty;
        var normalizedSearchTerm = originalSearchTerm.Length > 0 ? StringHelper.NormalizeVietnamese(originalSearchTerm) : string.Empty;

        // Tách thành các từ khóa con
        var originalTerms = originalSearchTerm.Length > 0 ?
            originalSearchTerm.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries) :
            Array.Empty<string>();
        var normalizedTerms = normalizedSearchTerm.Length > 0 ?
            normalizedSearchTerm.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries) :
            Array.Empty<string>();

        // Tạo truy vấn cơ bản
        var flashcards = _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true && x.DeletedAt == null)
            .AsQueryable();

        // Thêm điều kiện tìm kiếm nếu có searchTerm
        if (!string.IsNullOrEmpty(searchTerm))
        {
            flashcards = flashcards.Where(x =>
            // 1. Tìm kiếm trong tên theo cụm từ gốc
            x.FlashcardName.ToLower().Contains(originalSearchTerm) ||

            // 2. Tìm kiếm trong mô tả theo cụm từ gốc
            (x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(originalSearchTerm)) ||

            // 3. Tìm kiếm trong tên theo cụm từ được chuẩn hóa
            x.FlashcardName.ToLower().Contains(normalizedSearchTerm) ||

            // 4. Tìm kiếm trong mô tả theo cụm từ được chuẩn hóa 
            (x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(normalizedSearchTerm)) ||

            // 5. Tìm kiếm tất cả các từ (có dấu) trong tên
            originalTerms.All(term => x.FlashcardName.ToLower().Contains(term)) ||

            // 6. Tìm kiếm tất cả các từ (có dấu) trong mô tả
            (x.FlashcardDescription != null && originalTerms.All(term => x.FlashcardDescription.ToLower().Contains(term))) ||

            // 7. Tìm kiếm tất cả các từ (không dấu) trong tên - sử dụng hàm DB
            normalizedTerms.All(term => _context.NormalizeVietnameseDb(x.FlashcardName).Contains(term)) ||

            // 8. Tìm kiếm tất cả các từ (không dấu) trong mô tả - sử dụng hàm DB
            (x.FlashcardDescription != null && normalizedTerms.All(term => _context.NormalizeVietnameseDb(x.FlashcardDescription).Contains(term))) ||

            // 9. Tìm kiếm trong tags
            x.FlashcardTags.Any(ft =>
                // Tìm theo tag name gốc chứa cụm từ gốc
                ft.Tag.Name.ToLower().Contains(originalSearchTerm) ||

                // Tìm theo tag name chuẩn hóa chứa cụm từ chuẩn hóa
                ft.Tag.NormalizedName.Contains(normalizedSearchTerm) ||

                // Tìm theo tag name gốc chứa tất cả các từ gốc
                originalTerms.All(term => ft.Tag.Name.ToLower().Contains(term)) ||
                // Tìm theo tag name chuẩn hóa chứa tất cả các từ chuẩn hóa
                normalizedTerms.All(term => ft.Tag.NormalizedName.Contains(term)))
        );
        }

        // Lọc theo tags nếu có
        if (tags != null && tags.Any())
        {
            // Chuyển tags về chữ thường để tìm kiếm không phân biệt hoa thường
            var lowerTags = tags.Select(t => t.ToLower()).ToList();

            // Lọc flashcards có chứa tất cả tags được chỉ định
            flashcards = flashcards.Where(f =>
                lowerTags.All(tag =>
                    f.FlashcardTags.Any(ft =>
                        ft.Tag.Name.ToLower() == tag ||
                        ft.Tag.NormalizedName.ToLower() == tag)));
        }

        // Thêm include cần thiết
        flashcards = flashcards
        .Include(f => f.FlashcardContents)
        .Include(f => f.FlashcardTags)
            .ThenInclude(ft => ft.Tag)
        .Include(f => f.Subject)
        .Include(f => f.SubjectCurriculum)
            .ThenInclude(sc => sc.Subject)
        .Include(f => f.Chapter)
            .ThenInclude(c => c.SubjectCurriculum)
            .ThenInclude(sc => sc.Subject)
        .Include(f => f.Lesson)
            .ThenInclude(l => l.Chapter)
            .ThenInclude(c => c.SubjectCurriculum)
            .ThenInclude(sc => sc.Subject);


        // Sắp xếp theo mức độ phù hợp
        flashcards = flashcards
            .OrderByDescending(x => x.FlashcardName.ToLower().Contains(originalSearchTerm) ? 10 : 0)
            .ThenByDescending(x => x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(originalSearchTerm) ? 9 : 0)
            .ThenByDescending(x => x.FlashcardTags.Any(ft => ft.Tag.Name.ToLower().Contains(originalSearchTerm)) ? 8 : 0)
            .ThenByDescending(x => x.FlashcardName.ToLower().Contains(normalizedSearchTerm) ? 7 : 0)
            .ThenByDescending(x => x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(normalizedSearchTerm) ? 6 : 0)
            .ThenByDescending(x => x.FlashcardTags.Any(ft => ft.Tag.NormalizedName.Contains(normalizedSearchTerm)) ? 5 : 0)
            .ThenByDescending(x => originalTerms.All(term => x.FlashcardName.ToLower().Contains(term)) ? 4 : 0)
            .ThenByDescending(x => x.FlashcardDescription != null && originalTerms.All(term => x.FlashcardDescription.ToLower().Contains(term)) ? 3 : 0)
            .ThenByDescending(x => x.FlashcardTags.Any(ft => originalTerms.All(term => ft.Tag.Name.ToLower().Contains(term))) ? 2 : 0)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        // Thực hiện query
        var result = await flashcards.ToListAsync(cancellationToken);

        foreach (var flashcard in result)
        {
            await LoadRelatedEntities(flashcard, cancellationToken);
        }

        return result;
    }

    public async Task<IEnumerable<Flashcard>> SearchFlashcardsFullTextManagement(
        string searchTerm,
        int pageNumber,
        int pageSize,
        Guid? userId,
        bool? isCreatedBySystem,
        string? status,
        bool? isDeleted,
        List<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(searchTerm) && (tags == null || !tags.Any()))
            return await GetFlashcardsPlaceholder();

        var originalSearchTerm = searchTerm?.ToLower()?.Trim() ?? string.Empty;
        var normalizedSearchTerm = originalSearchTerm.Length > 0 ? StringHelper.NormalizeVietnamese(originalSearchTerm) : string.Empty;

        var originalTerms = originalSearchTerm.Length > 0 ?
            originalSearchTerm.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries) :
            Array.Empty<string>();
        var normalizedTerms = normalizedSearchTerm.Length > 0 ?
            normalizedSearchTerm.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries) :
            Array.Empty<string>();

        var flashcards = _entities.AsNoTracking().Where(x => x.Created)
            .AsQueryable();

        if (userId != Guid.Empty && userId != null)
        {
            flashcards = flashcards.Where(x => x.UserId == userId);
        }

        if (isCreatedBySystem.HasValue)
        {
            flashcards = flashcards.Where(x => x.IsCreatedBySystem == isCreatedBySystem);
        }

        if (!string.IsNullOrEmpty(status) && (status.Equals(StatusConstrains.OPEN)
                                                        || status.Equals(StatusConstrains.CLOSE)
                                                        || status.Equals(StatusConstrains.HIDDEN)
                                                        || status.Equals(StatusConstrains.ONLYLINK)))
        {
            flashcards = flashcards.Where(x => x.Status.Equals(status));
        }

        if (isDeleted.HasValue)
        {
            flashcards = isDeleted.Value ? flashcards.Where(x => x.DeletedAt != null) : flashcards.Where(x => x.DeletedAt == null);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            flashcards = flashcards.Where(x =>
            // 1. Tìm kiếm trong tên theo cụm từ gốc
            x.FlashcardName.ToLower().Contains(originalSearchTerm) ||

            // 2. Tìm kiếm trong mô tả theo cụm từ gốc
            (x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(originalSearchTerm)) ||

            // 3. Tìm kiếm trong tên theo cụm từ được chuẩn hóa
            x.FlashcardName.ToLower().Contains(normalizedSearchTerm) ||

            // 4. Tìm kiếm trong mô tả theo cụm từ được chuẩn hóa 
            (x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(normalizedSearchTerm)) ||

            // 5. Tìm kiếm tất cả các từ (có dấu) trong tên
            originalTerms.All(term => x.FlashcardName.ToLower().Contains(term)) ||

            // 6. Tìm kiếm tất cả các từ (có dấu) trong mô tả
            (x.FlashcardDescription != null && originalTerms.All(term => x.FlashcardDescription.ToLower().Contains(term))) ||

            // 7. Tìm kiếm tất cả các từ (không dấu) trong tên - sử dụng hàm DB
            normalizedTerms.All(term => _context.NormalizeVietnameseDb(x.FlashcardName).Contains(term)) ||

            // 8. Tìm kiếm tất cả các từ (không dấu) trong mô tả - sử dụng hàm DB
            (x.FlashcardDescription != null && normalizedTerms.All(term => _context.NormalizeVietnameseDb(x.FlashcardDescription).Contains(term))) ||

            // 9. Tìm kiếm trong tags
            x.FlashcardTags.Any(ft =>
                // Tìm theo tag name gốc chứa cụm từ gốc
                ft.Tag.Name.ToLower().Contains(originalSearchTerm) ||

                // Tìm theo tag name chuẩn hóa chứa cụm từ chuẩn hóa
                ft.Tag.NormalizedName.Contains(normalizedSearchTerm) ||

                // Tìm theo tag name gốc chứa tất cả các từ gốc
                originalTerms.All(term => ft.Tag.Name.ToLower().Contains(term)) ||
                // Tìm theo tag name chuẩn hóa chứa tất cả các từ chuẩn hóa
                normalizedTerms.All(term => ft.Tag.NormalizedName.Contains(term)))
        );
        }

        // Lọc theo tags nếu có
        if (tags != null && tags.Any())
        {
            // Chuyển tags về chữ thường để tìm kiếm không phân biệt hoa thường
            var lowerTags = tags.Select(t => t.ToLower()).ToList();

            // Lọc flashcards có chứa tất cả tags được chỉ định
            flashcards = flashcards.Where(f =>
                lowerTags.All(tag =>
                    f.FlashcardTags.Any(ft =>
                        ft.Tag.Name.ToLower() == tag ||
                        ft.Tag.NormalizedName.ToLower() == tag)));
        }

        // Thêm include cần thiết
        flashcards = flashcards
            .Include(f => f.FlashcardContents)
            .Include(f => f.FlashcardTags)
                .ThenInclude(ft => ft.Tag)
            .Include(f => f.Subject)
            .Include(f => f.SubjectCurriculum)
            .Include(f => f.Chapter)
            .Include(f => f.Lesson);

        // Sắp xếp theo mức độ phù hợp
        flashcards = flashcards
            .OrderByDescending(x => x.FlashcardName.ToLower().Contains(originalSearchTerm) ? 10 : 0)
            .ThenByDescending(x => x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(originalSearchTerm) ? 9 : 0)
            .ThenByDescending(x => x.FlashcardTags.Any(ft => ft.Tag.Name.ToLower().Contains(originalSearchTerm)) ? 8 : 0)
            .ThenByDescending(x => x.FlashcardName.ToLower().Contains(normalizedSearchTerm) ? 7 : 0)
            .ThenByDescending(x => x.FlashcardDescription != null && x.FlashcardDescription.ToLower().Contains(normalizedSearchTerm) ? 6 : 0)
            .ThenByDescending(x => x.FlashcardTags.Any(ft => ft.Tag.NormalizedName.Contains(normalizedSearchTerm)) ? 5 : 0)
            .ThenByDescending(x => originalTerms.All(term => x.FlashcardName.ToLower().Contains(term)) ? 4 : 0)
            .ThenByDescending(x => x.FlashcardDescription != null && originalTerms.All(term => x.FlashcardDescription.ToLower().Contains(term)) ? 3 : 0)
            .ThenByDescending(x => x.FlashcardTags.Any(ft => originalTerms.All(term => ft.Tag.Name.ToLower().Contains(term))) ? 2 : 0)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        // Thực hiện query
        var result = await flashcards.ToListAsync(cancellationToken);

        foreach (var flashcard in result)
        {
            await LoadRelatedEntities(flashcard, cancellationToken);
        }

        return result;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsByLessonId(Guid? lessonId,
        CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking()
            .Where(x => x.LessonId.Equals(lessonId) && x.Created == true)
            .Include(l => l.Lesson)
            .ThenInclude(c => c.Chapter)
            .OrderByDescending(x => x.CreatedAt)
            .Take(4)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsByChapterId(Guid? chapterId,
        CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking()
            .Where(x => x.ChapterId.Equals(chapterId) && x.Created == true)
            .Include(c => c.Chapter)
            .ThenInclude(sc => sc.SubjectCurriculum)
            .OrderByDescending(x => x.CreatedAt)
            .Take(4)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsBySubjectCurriculumId(Guid? subjectCurriculumId,
        CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking()
            .Where(x => x.SubjectCurriculumId.Equals(subjectCurriculumId) && x.Created == true)
            .Include(sc => sc.SubjectCurriculum)
            .ThenInclude(s => s.Subject)
            .OrderByDescending(x => x.CreatedAt)
            .Take(4)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Flashcard>> GetFlashcardsByMasterSubjectId(Guid masterSubjectId, int? grade, CancellationToken cancellationToken)
    {
        var query = _entities.AsNoTracking()
            .Where(f => f.Subject != null && f.Subject.MasterSubjectId == masterSubjectId &&
            f.Created == true && f.Status == StatusConstrains.OPEN && f.DeletedAt ==null).Include(f=>f.FlashcardContents).AsQueryable();

        if (grade.HasValue)
        {
            query = query.Where(f => f.Subject.Category == "Grade" + grade.Value.ToString());
        }

        return await query
            .Include(f => f.Subject)
            .ThenInclude(s => s.MasterSubject)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Flashcard>> GetFlashcardsPlaceholder(int? grade = null)
    {
        var query = _entities.Where(f=>f.Created == true && f.Status == StatusConstrains.OPEN
        && f.DeletedAt == null).Include(f=>f.FlashcardContents).AsQueryable();

        if (grade.HasValue)
        {
            query = query.Where(f => f.Subject.Category == "Grade" + grade.Value.ToString());
        }

        return await query
            .OrderByDescending(f => f.TotalView ?? 0)
            .Take(8)
            .Include(f => f.Subject)
            .ThenInclude(s => s.MasterSubject)
            .ToListAsync();
    }
    public async Task<List<Flashcard>> GetFlashcardsPlaceholderNoGrade(int numberMissing)
    {
        var query = _entities.Where(f => f.Created == true && f.Status == StatusConstrains.OPEN
        && f.DeletedAt == null).Include(f => f.FlashcardContents).AsQueryable();

        return await query
            .OrderByDescending(f => f.TotalView ?? 0)
            .Take(numberMissing)
            .Include(f => f.Subject)
            .ThenInclude(s => s.MasterSubject)
            .ToListAsync();
    }
}