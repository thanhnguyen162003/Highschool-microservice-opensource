using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class SubjectCurriculumRepository(DocumentDbContext context) : BaseRepository<SubjectCurriculum>(context), ISubjectCurriculumRepository
{
    private readonly DocumentDbContext _context = context;

    public async Task<bool> AddSubjectCurriculum(SubjectCurriculum subjectCurriculum, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(subjectCurriculum, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
    public async Task<List<Guid>> GetSubjectCurriculumIdBySubjectId(IEnumerable<string> subjectIds)
    {
        if (subjectIds == null || !subjectIds.Any())
        {
            return new List<Guid>();
        }

        List<Guid> guids = subjectIds.Select(guid => Guid.Parse(guid)).ToList();
        return await _entities
            .AsNoTracking()
            .Where(e => guids.Contains(e.SubjectId))
            .Select(e => e.Id)
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
        var result = await _entities.Where(x => x.SubjectId == subjectId && x.CurriculumId == curriculumId && x.IsPublish == true)
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

    public IEnumerable<CourseQueryModel> GetSubjectCurriculums()
    {
        var result = from subjectCurriculum in _entities.AsNoTracking().AsSplitQuery()
                     join subject in _context.Subjects.AsNoTracking().AsSplitQuery() on subjectCurriculum.SubjectId equals subject.Id
                     select new CourseQueryModel
                     {
                         SubjectId = subject.Id,
                         SubjectName = subject.SubjectName,
                         SubjectCurriculumId = subjectCurriculum.Id,
                         SubjectCurriculumName = subjectCurriculum.SubjectCurriculumName,
                         Type = SearchCourseType.SubjectCurriculum.ToString(),
                         Name = subjectCurriculum.SubjectCurriculumName ?? string.Empty
                     };

        return result;
    }

	public Task<List<SubjectCurriculum>> GetSubjectCurriculaBySubjectId(Guid subjectId, CancellationToken cancellationToken = default)
	{
		return _entities
			.AsNoTracking()
			.Where(x => x.SubjectId == subjectId)
            .Include(x => x.Curriculum)
			.ToListAsync(cancellationToken);
	}

	public Task<bool> UpdateSubjectCurriculum(SubjectCurriculum subjectCurriculum, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<List<SubjectCurriculum>> GetSubjectCurriculumOfCurriculum(Guid curriculumId, CancellationToken cancellationToken = default)
	{
		var subjectCurriculum = await _entities.Where(x => x.CurriculumId == curriculumId).ToListAsync();
		return subjectCurriculum;
	}

    public async Task<IEnumerable<string>> CheckSubjectCurriculumName(IEnumerable<string> subjectCurriculumIds)
    {
        var existingSubjectNamesInDb = await _context.SubjectCurricula
            .Where(x => (bool)x.IsPublish!)
            .Select(x => x.Id.ToString())
            .ToListAsync();

        var nonExistingSubjectNames = subjectCurriculumIds.Except(existingSubjectNamesInDb);

        return nonExistingSubjectNames;

    }

    public async Task<Dictionary<string,int>> GetSubjectCurriculumCount(CancellationToken cancellationToken = default)
    {
        var counts = await _entities
            .AsNoTracking()
            .GroupBy(x => x.IsPublish)
            .Select(g => new { IsPublish = g.Key, Count = g.Count() })
            .ToListAsync();

        var result = new Dictionary<string, int>
        {
            { "Publish", counts.FirstOrDefault(x => x.IsPublish == true)?.Count ?? 0 },
            { "UnPublish", counts.FirstOrDefault(x => x.IsPublish == false)?.Count ?? 0 }
        };

        return result;
    }

    public async Task<bool> IsSubjectCurriculumPublish(Guid subjectId, Guid curriculumId, CancellationToken cancellationToken = default)
	{
		var subjectCurriculum = await _entities
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.SubjectId == subjectId && x.CurriculumId == curriculumId, cancellationToken);
		if (subjectCurriculum == null)
        {
			return false;
		}
		if (subjectCurriculum.IsPublish == true)
		{
            return true;
		}
		return false;
	}
}