using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class LessonRepository(DocumentDbContext context) : BaseRepository<Lesson>(context), ILessonRepository
{
    private readonly DocumentDbContext _context = context;

    public async Task SoftDelete(IEnumerable<Guid> guids)
    {
        await _entities.Where(x => guids.Any(id => id == x.Id)).ForEachAsync(x => x.DeletedAt = DateTime.Now);
    }

    public async Task<Lesson?> GetById(Guid lessonId)
    {
        return await _entities
            .AsNoTracking()
            .Include(x => x.Theories)
            .Include(x => x.Chapter).ThenInclude(x => x.SubjectCurriculum)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == lessonId && x.DeletedAt == null);
    }

    public async Task<(List<Lesson> Lessons, int TotalCount)> GetLessonsByFilters(Guid? chapterId, LessonQueryFilter queryFilter)
    {
        var lessons = _entities
            .AsNoTracking()
            .Include(x => x.Theories)
            .Include(x => x.Chapter).ThenInclude(x => x.SubjectCurriculum)
            .AsQueryable();
        lessons = ApplyFilterSortAndSearch(lessons, queryFilter, chapterId);
        int totalCount = await _entities.CountAsync();
        var result =  await lessons.ToListAsync();
        return (result, totalCount);
    }

    public async Task<bool> UpdateListLessons(List<Lesson?> lessons)
    {
        try
        {
            _entities.UpdateRange(lessons);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LessonIdExistAsync(Guid? lessonId)
    {
        return await _entities.AnyAsync(x => x.Id == lessonId);
    }

    public async Task<bool> AddSingleLesson(Lesson lesson)
    {
        await _entities.AddAsync(lesson);
        var result = await context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }
    
    public async Task<bool> UpdateLesson(Lesson lesson)
    {
         _entities.Update(lesson);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> CreateListLessons(List<Lesson> lessons)
    {
        try
        {
            await _entities.AddRangeAsync(lessons);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private IQueryable<Lesson> ApplyFilterSortAndSearch(IQueryable<Lesson> lessons, LessonQueryFilter queryFilter, Guid? chapterId)
    {
        lessons = lessons.OrderBy(c => c.DisplayOrder);
        if (chapterId != null)
        {
            lessons = lessons.Where(x => x.ChapterId == chapterId);
        }

        lessons = lessons.Where(x => x.DeletedAt == null);

        return lessons;
    }

    public async Task<IEnumerable<string>> GetLessonBySubjectId(IEnumerable<string> subjectIds, CancellationToken cancellationToken = default)
    {
        var lesson = await _entities
            .AsNoTracking()
            .Where(x => subjectIds.Contains(x.Chapter.SubjectCurriculum.SubjectId.ToString()))
            .Select(x => x.Id.ToString())
            .ToListAsync();
        return (IEnumerable<string>)lesson;
    }
}