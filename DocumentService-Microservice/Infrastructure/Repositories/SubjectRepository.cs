using Domain.CustomModel;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using System.Linq;

namespace Infrastructure.Repositories;

public class SubjectRepository(DocumentDbContext context) : BaseRepository<Subject>(context), ISubjectRepository
{
    private readonly DocumentDbContext _context = context;

    public async Task<List<Subject>> GetSubjects(CancellationToken cancellationToken = default)
	{
		return await _entities
            .AsNoTracking()
            .Include(x => x.SubjectCurricula)
            .ThenInclude(y=>y.Chapters).Where(chapter => chapter.DeletedAt.Equals(null))
            .AsSplitQuery()
            .Include(s=>s.Category)
			.ToListAsync(cancellationToken);
	}

	public async Task<(List<Subject> Subjects, int TotalCount)> GetSubjects(SubjectQueryFilter queryFilter, CancellationToken cancellationToken = default)
    {
        var subjects = _entities
            .AsNoTracking()
            .Include(x => x.SubjectCurricula)
            .ThenInclude(y=>y.Chapters).Where(chapter => chapter.DeletedAt.Equals(null))
            .AsSplitQuery()
            .Include(s=>s.Category)
            .AsQueryable();
        
        subjects = ApplyFilterSortAndSearch(subjects, queryFilter);
        subjects = GetByClassFilter(subjects, queryFilter);
        int totalCount = await subjects.CountAsync();
        
        subjects = subjects
            .OrderBy(y => y.Category.CategoryName) 
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        
        var pagination = await subjects.ToListAsync(cancellationToken);
        return (pagination, totalCount);
    }
    
    public async Task<List<Subject>> GetSubjectsRelatedClass(string className, CancellationToken cancellationToken = default)
    {
        var subjects = _entities
            .AsNoTracking()
            .Where(x => x.DeletedAt == null && x.Category.CategoryName.ToLower() == className.ToLower())
            .OrderBy(y => y.Category.CategoryName)
            .Take(3); // Limit the number of records

        // Return only the list of Ids
        return subjects.ToList();
    }

    public async Task<SubjectModel> GetSubjectBySubjectId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking()
            .Where(x => x.Id == id && x.DeletedAt == null)
            .Select(x => new SubjectModel
            {
                Id = x.Id,
                SubjectName = x.SubjectName,
                SubjectDescription = x.SubjectDescription,
                CategoryName = x.Category.CategoryName,
                Information = x.Information,
                Image = x.Image,
                Slug = x.Slug,
                Like = x.Like,
                SubjectCode = x.SubjectCode,
                View = x.View,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                IsLike = false
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SubjectModel> GetSubjectUnPublishBySubjectId(Guid id, CancellationToken cancellationToken = default)
    {
        return (await _entities.AsNoTracking().Where(
            x => x.Id.Equals(id) && x.DeletedAt.Equals(null)).Select(x => new SubjectModel()
        {
            Id = x.Id,
            SubjectName = x.SubjectName,
            SubjectDescription = x.SubjectDescription,
            // Class = x.Class,
            CategoryName = x.Category.CategoryName,
            Information = x.Information,
            Image = x.Image,
            Slug = x.Slug,
            Like = x.Like,
            SubjectCode = x.SubjectCode,
            View = x.View,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).FirstOrDefaultAsync(cancellationToken))!;
    }

    public async Task<SubjectModel> GetSubjectBySubjectSlug(string slug, CancellationToken cancellationToken = default)
    {
        return (await _entities.AsNoTracking().Where(
            x => x.Slug.Equals(slug) && x.DeletedAt.Equals(null)).Select(x => new SubjectModel()
        {
            Id = x.Id,
            SubjectName = x.SubjectName,
            SubjectDescription = x.SubjectDescription,
            // Class = x.Class,
            Information = x.Information,
            Image = x.Image,
            CategoryName = x.Category.CategoryName,
            Slug = x.Slug,
            Like = x.Like,
            SubjectCode = x.SubjectCode,
            View = x.View,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).FirstOrDefaultAsync(cancellationToken))!;
    }

    public Task<SubjectModel> GetSubjectBySubjectSlugAndCurriculum(string slug, Guid curriculumId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<SubjectModel> GetSubjectUnPublishBySubjectSlug(string slug, CancellationToken cancellationToken = default)
    {
        return (await _entities.AsNoTracking().Where(
            x => x.Slug.Equals(slug) && x.DeletedAt.Equals(null)).Select(x => new SubjectModel()
        {
            Id = x.Id,
            SubjectName = x.SubjectName,
            SubjectDescription = x.SubjectDescription,
            // Class = x.Class,
            Information = x.Information,
            Image = x.Image,
            CategoryName = x.Category.CategoryName,
            Slug = x.Slug,
            Like = x.Like,
            SubjectCode = x.SubjectCode,
            View = x.View,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).FirstOrDefaultAsync(cancellationToken))!;
    }

    public async Task<bool> DeleteSubject(Guid id, CancellationToken cancellationToken = default)
    {
        var subject = await _entities.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
        if (subject == null)
        {
            return false;
        }
        subject.DeletedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    

    public async Task<bool> UpdateSubject(Subject dto, CancellationToken cancellationToken = default)
    {
        var subject = await _context.Subjects.FirstOrDefaultAsync(x => x.Id.Equals(dto.Id), cancellationToken);
        if (subject == null)
        {
            return false;
        }
        subject.SubjectName = dto.SubjectName ?? subject.SubjectName;
        subject.SubjectCode = dto.SubjectCode ?? subject.SubjectCode;
        subject.Information = dto.Information ?? subject.Information;
        subject.Image = dto.Image ?? subject.Image;
        subject.CategoryId = dto.CategoryId ?? subject.CategoryId;
        // subject.Class = dto.Class ?? subject.Class;
        subject.SubjectDescription = dto.SubjectDescription ?? subject.SubjectDescription;
        subject.Slug = dto.Slug ?? subject.Slug;
        subject.UpdatedAt = DateTime.Now;
        _entities.Update(subject);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CreateSubject(Subject subject, CancellationToken cancellationToken = default)
    {
        _context.Subjects.Add(subject);
        var result = await _context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        return false;
    }
    
    public async Task<IEnumerable<Subject>> GetSubjectByClass(string classes, SubjectQueryFilter queryFilter)
    {
        var subjects = _entities
            .AsNoTracking()
            .Where(x=>x.Category.CategoryName.ToLower().Equals(classes.ToLower()))
            .Include(s=>s.SubjectCurricula)
            .ThenInclude(x => x.Chapters.Where(y=>y.DeletedAt.Equals(null))).AsQueryable();
        subjects = ApplyFilterSortAndSearch(subjects, queryFilter);
        return await subjects.ToListAsync();
    }

    public async Task<bool> SubjectIdExistAsync(Guid? guid)
    {
        return await _entities.AnyAsync(x => x.Id.Equals(guid));
    }

    public async Task<bool> SubjectNameExistAsync(string name)
    {
        return await _entities.AnyAsync(x => x.SubjectName.Equals(name));
    }
    
    private IQueryable<Subject> ApplyFilterSortAndSearch(IQueryable<Subject> subjects, SubjectQueryFilter queryFilter)
    {
        subjects = subjects.Where(x => x.DeletedAt.Equals(null));
        
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            subjects = subjects.Where(x => x.SubjectName.ToLower().Contains(queryFilter.Search.ToLower()));
        }
        return subjects;
    }
    private IQueryable<Subject> GetByClassFilter(IQueryable<Subject> subjects, SubjectQueryFilter queryFilter)
    {
        if (!string.IsNullOrEmpty(queryFilter.Class))
        {
            subjects = subjects.Where(x => x.DeletedAt.Equals(null) && x.Category.CategoryName.ToLower().Equals(queryFilter.Class.ToLower()));
        }
        return subjects;
    }
    
    public async Task<IEnumerable<string>> CheckSubjectName(IEnumerable<string> subjectIds)
    {
		var existingSubjectNamesInDb = await _context.Subjects
			.Where(x => x.DeletedAt == null)
			.Select(x => x.Id.ToString())
			.ToListAsync();

		var nonExistingSubjectNames = subjectIds.Except(existingSubjectNamesInDb);

		return nonExistingSubjectNames;

	}


    public async Task<List<Subject>> GetPlaceHolderSubjects()
    {
        var subjects = await _entities
            .AsNoTracking()
            .Take(4)
            .Include(s=>s.Category)
            .ToListAsync();
        return subjects;
    }

    public async Task UpdateViewCount(Guid id)
    {
        var subject = await _entities.FirstOrDefaultAsync(x => x.Id.Equals(id));
        subject.View++;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Subject>> GetAdditionalSubjects(string className, int count, CancellationToken cancellationToken)
    {
        return await _context.Subjects
            .Where(s => s.Category.CategoryName.Equals(className) && s.DeletedAt.Equals(null))
            .OrderBy(s => Guid.NewGuid()) // Randomize the order
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetSubjectIdsAsString(CancellationToken cancellationToken = default)
    {
        return await _entities
            .AsNoTracking()
            .OrderByDescending(x => x.View)
            .Select(s => s.Id.ToString()).Take(2) // Convert Guid to string
            .ToListAsync(cancellationToken);
    }
    public async Task<Dictionary<string,string>> GetGrade(List<string> subjectId, CancellationToken cancellationToken = default)
    {
        var subject = _context.Subjects
            .AsNoTracking()
            .Where(x => subjectId.Contains(x.Id.ToString()))
            .Where(x => x.DeletedAt == null)
            .Include(x => x.Category)
            .ToList();

        // Return only the list of Ids

        Dictionary<string, string> result = new Dictionary<string, string>();
        if (subject != null)
        {
            foreach (var item in subject)
            {
                result.Add(item.Id.ToString(), item.Category.CategoryName);
            }
            
        }
        return result;
    }
}