using Domain.CustomModel;
using Domain.Enums;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class ChapterRepository(DocumentDbContext context) : BaseRepository<Chapter>(context), IChapterRepository
{
    private readonly DocumentDbContext _context = context;

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChapters(ChapterQueryFilter queryFilter)
    {
        var chapters = _entities
            .AsNoTracking()
            .Where(c=>c.SubjectCurriculum.IsPublish == true)
            .Include(x=>x.SubjectCurriculum)
            .ThenInclude(c=>c.Curriculum)
            .AsSplitQuery()
            .AsQueryable();
        chapters = ApplyFilterSortAndSearch(chapters, queryFilter);
        int totalCount = await chapters.CountAsync();
        chapters = chapters.OrderBy(y => y.ChapterLevel)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize).Include(x => x.SubjectCurriculum);
        var pagination = await chapters.ToListAsync();
        var sortedChapters = pagination
            .OrderBy(c =>
            {
                // Try to parse ChapterLevel; if it fails, assign int.MaxValue to sort it at the end
                return int.TryParse(c.ChapterLevel, out int level) ? level : int.MaxValue;
            })
            .ToList();
        return (pagination, totalCount);
    }
    
    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersModerator(ChapterQueryFilter queryFilter)
    {
        var chapters = _entities
            .AsNoTracking()
            .Include(x=>x.SubjectCurriculum)
            .ThenInclude(c=>c.Curriculum)
            .AsSplitQuery()
            .AsQueryable();
        chapters = ApplyFilterSortAndSearch(chapters, queryFilter);
        int totalCount = await chapters.CountAsync();
        chapters = chapters.OrderBy(y => y.ChapterLevel)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize).Include(x => x.SubjectCurriculum);
        var pagination = await chapters.ToListAsync();
        var sortedChapters = pagination
            .OrderBy(c =>
            {
                // Try to parse ChapterLevel; if it fails, assign int.MaxValue to sort it at the end
                return int.TryParse(c.ChapterLevel, out int level) ? level : int.MaxValue;
            })
            .ToList();
        return (pagination, totalCount);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubject(ChapterQueryFilter queryFilter,Guid subjectId)
    {
        var chapters = _entities
            .Where(x=>x.SubjectCurriculum.SubjectId.Equals(subjectId))
            .Include(l=>l.Lessons).AsNoTracking();
        chapters = ApplyFilterSortAndSearch(chapters, queryFilter);
        int totalCount = await chapters.CountAsync();
        chapters = chapters.Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        var pagination = await chapters.ToListAsync();
        var sortedChapters = pagination
            .OrderBy(c =>
            {
                // Try to parse ChapterLevel; if it fails, assign int.MaxValue to sort it at the end
                return int.TryParse(c.ChapterLevel, out int level) ? level : int.MaxValue;
            })
            .ToList();
        return (pagination, totalCount);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumId(ChapterQueryFilter queryFilter, Guid subjectCurriculumId)
    {
        var chapters = _entities
            .Where(x=>x.SubjectCurriculum.Id.Equals(subjectCurriculumId) && x.SubjectCurriculum.IsPublish == true)
            .Include(l=>l.Lessons)
            .Include(x=>x.SubjectCurriculum)
            .ThenInclude(c=>c.Curriculum)
            .AsSplitQuery().AsNoTracking();
        chapters = ApplyFilterSortAndSearch(chapters, queryFilter);
        int totalCount = await chapters.CountAsync();
        chapters = chapters.Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        var pagination = await chapters.ToListAsync();
        var sortedChapters = pagination
            .OrderBy(c =>
            {
                return int.TryParse(c.ChapterLevel, out int level) ? level : int.MaxValue;
            })
            .ToList();
        return (pagination, totalCount);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumIdModerator(ChapterQueryFilter queryFilter, Guid subjectCurriculumId)
    {
        var chapters = _entities
            .Where(x=>x.SubjectCurriculum.Id.Equals(subjectCurriculumId))
            .Include(l=>l.Lessons)
            .Include(x=>x.SubjectCurriculum)
            .ThenInclude(c=>c.Curriculum)
            .AsSplitQuery().AsNoTracking();
        chapters = ApplyFilterSortAndSearch(chapters, queryFilter);
        int totalCount = await chapters.CountAsync();
        chapters = chapters.Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        var pagination = await chapters.ToListAsync();
        var sortedChapters = pagination
            .OrderBy(c =>
            {
                return int.TryParse(c.ChapterLevel, out int level) ? level : int.MaxValue;
            })
            .ToList();
        return (pagination, totalCount);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculum(ChapterQueryFilter queryFilter, Guid curriculumId, Guid subjectId)
    {
        var chapters = _entities
            .Where(x=>x.SubjectCurriculum.CurriculumId.Equals(curriculumId) && x.SubjectCurriculum.SubjectId.Equals(subjectId) && x.SubjectCurriculum.IsPublish == true)
            .Include(l=>l.Lessons).AsNoTracking();
        chapters = ApplyFilterSortAndSearch(chapters, queryFilter);
        int totalCount = await chapters.CountAsync();
        chapters = chapters.Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        var pagination = await chapters.ToListAsync();
        var sortedChapters = pagination
            .OrderBy(c =>
            {
                return int.TryParse(c.ChapterLevel, out int level) ? level : int.MaxValue;
            })
            .ToList();
        return (pagination, totalCount);
    }

    public async Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumModerator(ChapterQueryFilter queryFilter, Guid curriculumId, Guid subjectId)
    {
        var chapters = _entities
            .Where(x=>x.SubjectCurriculum.CurriculumId.Equals(curriculumId) &&
            x.SubjectCurriculum.SubjectId.Equals(subjectId))
            .Include(l=>l.Lessons).AsNoTracking();
        chapters = ApplyFilterSortAndSearch(chapters, queryFilter);
        int totalCount = await chapters.CountAsync();
        chapters = chapters.Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        var pagination = await chapters.ToListAsync();
        var sortedChapters = pagination
            .OrderBy(c =>
            {
                return int.TryParse(c.ChapterLevel, out int level) ? level : int.MaxValue;
            })
            .ToList();
        return (pagination, totalCount);
    }

    public async Task<ChapterModel> GetChapterByChapterId(Guid id)
    {
        return (await _entities.AsNoTracking()
            .Where(c => c.Id.Equals(id) && c.DeletedAt.Equals(null))
            .Select(c => new ChapterModel()
            {
                Id = c.Id,
                ChapterName = c.ChapterName,
                ChapterLevel = c.ChapterLevel,
                SubjectId = c.SubjectCurriculum.SubjectId,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync())!;
    }
    
    public async Task<bool> DeleteChapter(Guid id)
    {
        var chapter = await _entities.Where(c => c.Id.Equals(id)).FirstOrDefaultAsync();
        if (chapter == null)
        {
            return false;
        }
        chapter.DeletedAt = DateTime.UtcNow;
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> UpdateChapter(Chapter chapter)
    {
        var chapterData = await _entities.FirstOrDefaultAsync(c => c.Id.Equals(chapter.Id));
        if (chapterData == null)
        {
            return false;
        }
        chapterData.ChapterName = chapter.ChapterName ?? chapterData.ChapterName;
        chapterData.ChapterLevel = chapter.ChapterLevel ?? chapterData.ChapterLevel;
        chapterData.Description = chapter.Description ?? chapterData.Description;
        chapterData.UpdatedAt = DateTime.UtcNow;
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> CreateChapter(Chapter chapter)
    {
        _entities.Add(chapter);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> CreateChapterList(List<Chapter> listChapters)
    {
        try
        {
            await _entities.AddRangeAsync(listChapters);
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

    public async Task<bool> ChapterIdExistsAsync(Guid? guid)
    {
        return await _entities.AnyAsync(s => s.Id == guid);
    }
    
    public async Task<bool> ChapterNameExistsAsync(string name)
    {
        return await _entities.AnyAsync(s => s.ChapterName.ToLower() == name.ToLower());
    }
    
    private IQueryable<Chapter> ApplyFilterSortAndSearch(IQueryable<Chapter> chapters, ChapterQueryFilter queryFilter)
    {
        chapters = chapters.OrderBy(c => c.ChapterLevel);
        chapters = chapters.Where(c => c.DeletedAt.Equals(null));
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            chapters = chapters.Where(c => c.ChapterName.Contains(queryFilter.Search));
        }
        return chapters;
    }

    public IEnumerable<CourseQueryModel> GetChapters()
    {
        var result = from chapter in _entities.AsNoTracking().AsSplitQuery()
                     join subjectCurriculum in _context.SubjectCurricula.AsNoTracking().AsSplitQuery() on chapter.SubjectCurriculumId equals subjectCurriculum.Id
                     join subject in _context.Subjects.AsNoTracking().AsSplitQuery() on subjectCurriculum.SubjectId equals subject.Id
                     select new CourseQueryModel
                     {
                         ChapterId = chapter.Id,
                         ChapterName = chapter.ChapterName,
                         SubjectId = subject.Id,
                         SubjectName = subject.SubjectName,
                         SubjectCurriculumId = subjectCurriculum.Id,
                         SubjectCurriculumName = subjectCurriculum.SubjectCurriculumName,
                         Type = SearchCourseType.Chapter.ToString(),
                         Name = chapter.ChapterName
                     };

        return result;
    }

}