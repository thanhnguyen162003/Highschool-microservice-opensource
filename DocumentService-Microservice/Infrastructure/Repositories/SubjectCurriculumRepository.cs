using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class SubjectCurriculumRepository(DocumentDbContext context) : BaseRepository<SubjectCurriculum>(context), ISubjectCurriculumRepository
{
    public async Task<bool> AddSubjectCurriculum(SubjectCurriculum subjectCurriculum, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(subjectCurriculum, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
    public async Task<List<string>> GetSubjectCurriculumIdBySubjectId(IEnumerable<string> subjectIds)
    {
        return await _entities
            .AsNoTracking()
            .Where(e => subjectIds.Contains(e.SubjectId.ToString()))
            .Select(e => e.Id.ToString())
            .ToListAsync();
    }
    public async Task<List<Curriculum>> GetCurriculumsOfSubject(Guid subjectId, CancellationToken cancellationToken = default)
    {
        return await _entities
            .AsNoTracking()
            .Where(sc => sc.SubjectId == subjectId)
            .Select(sc => sc.Curriculum)
            .ToListAsync(cancellationToken);
    }

    public async Task<SubjectCurriculum> GetSubjectCurriculumById(Guid subjectCurriculumId, CancellationToken cancellationToken = default)
    {
        var subjectCurriculum = await _entities.Where(x => x.Id == subjectCurriculumId && x.Subject.DeletedAt.Equals(null)).FirstOrDefaultAsync();
        return subjectCurriculum!;
    }

    public async Task<bool> IsSubjectCurriculumExists(Guid subjectId, Guid curriculumId, CancellationToken cancellationToken = default)
    {
        var result = await _entities.Where(x => x.SubjectId == subjectId && x.CurriculumId == curriculumId)
            .FirstOrDefaultAsync(cancellationToken);
        if (result is null)     
        {
            return false;
        }
        return true;
    }

    public async Task<SubjectCurriculum> GetSubjectCurriculum(Guid subjectId, Guid curriculumId, CancellationToken cancellationToken = default)
    {
        var result = await _entities.Where(x => x.SubjectId == subjectId && x.CurriculumId == curriculumId)
            .FirstOrDefaultAsync(cancellationToken);
        return result;
    }

    public async Task<(List<SubjectCurriculum> subjectCurricula, int TotalCount)> GetSubjectCurriculaPublish(SubjectCurriculumQueryFilter queryFilter, CancellationToken cancellationToken = default)
    {
        var subjects = _entities
            .AsNoTracking()
            .Where(x=>x.IsPublish == true)
            .Include(s=>s.Subject)
            .Include(cn=>cn.Curriculum)
            .AsSplitQuery()
            .AsQueryable();
        
        subjects = ApplyFilterSortAndSearch(subjects, queryFilter);
        int totalCount = await subjects.CountAsync();
        
        subjects = subjects
            .OrderBy(y => y.SubjectCurriculumName) 
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        
        var pagination = await subjects.ToListAsync(cancellationToken);
        return (pagination, totalCount);
    }

    public async Task<List<SubjectCurriculum>> GetSubjectCurriculaRelated(Guid curriculumId, CancellationToken cancellationToken = default)
    {
        var subjects = _entities
            .AsNoTracking()
            .Where(x => x.Subject != null && x.Subject.DeletedAt == null && x.Curriculum != null && x.Curriculum.Id == curriculumId);
        subjects = subjects.Take(3);

        return await subjects.ToListAsync(cancellationToken);
    }


    public async Task<bool> UnPublishSubjectCurriculum(Guid id, CancellationToken cancellationToken = default)
    {
        var subject = await _entities.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
        if (subject == null)
        {
            return false;
        }
        subject.IsPublish = false;
        var result = await context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> PublishSubjectCurriculum(Guid id, CancellationToken cancellationToken = default)
    {
        var subject = await _entities.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
        if (subject == null)
        {
            return false;
        }
        subject.IsPublish = true;
        var result = await context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<(List<SubjectCurriculum> subjectCurricula, int TotalCount)> GetSubjectCurriculaUnPublish(SubjectCurriculumQueryFilter queryFilter, CancellationToken cancellationToken = default)
    {
        var subjects = _entities
            .AsNoTracking()
            .Where(x=>x.IsPublish == false)
            .Include(s=>s.Subject)
            .Include(cn=>cn.Curriculum)
            .AsSplitQuery()
            .AsQueryable();
        
        subjects = ApplyFilterSortAndSearch(subjects, queryFilter);
        int totalCount = await subjects.CountAsync();
        
        subjects = subjects
            .OrderBy(y => y.SubjectCurriculumName) 
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        
        var pagination = await subjects.ToListAsync(cancellationToken);
        return (pagination, totalCount);
    }
    
    private IQueryable<SubjectCurriculum> ApplyFilterSortAndSearch(IQueryable<SubjectCurriculum> subjects, SubjectCurriculumQueryFilter queryFilter)
    {
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            subjects = subjects.Where(x => x.SubjectCurriculumName.ToLower().Contains(queryFilter.Search.ToLower()));
        }
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            subjects = subjects.Where(x => x.CurriculumId.Equals(queryFilter.SortCurriculum));
        }
        return subjects;
    }
}