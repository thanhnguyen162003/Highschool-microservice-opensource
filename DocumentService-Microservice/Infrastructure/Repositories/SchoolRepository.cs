using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class SchoolRepository(DocumentDbContext context) : BaseRepository<School>(context), ISchoolRepository
{
    public async Task<bool> CreateSchoolAsync(School school)
    {
        await _entities.AddAsync(school);
        var result = await context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteSchool(Guid id)
    {
        var school = await _entities.FindAsync(id);
        if (school is null)
        {
            return false;
        }
        school.DeletedAt = DateTime.UtcNow;
        _entities.Update(school);
        var result = await context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<(List<School> Schools, int TotalCount)> GetSchoolsAsync(SchoolQueryFilter queryFilter)
    {
        var schools = _entities
            .AsNoTracking()
            .Include(p=>p.Province)
            .Include(x => x.Documents.Where(m => m.DeletedAt.Equals(null))) 
            .AsQueryable();

        schools = ApplyFilterSortAndSearch(schools, queryFilter);
        int totalCount = await schools.CountAsync();
        schools = schools
            .OrderBy(y => y.SchoolName)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        var pagination = await schools.ToListAsync();
        return (pagination, totalCount);
    }

    public async Task<(List<School> Schools, int TotalCount)> GetSchoolsByProvinceIdAsync(int id, SchoolQueryFilter queryFilter)
    {
        var schools = _entities
            .AsNoTracking().Where(d=>d.ProvinceId == id)
            .Include(x => x.Documents.Where(m => m.DeletedAt.Equals(null))) 
            .AsQueryable();

        schools = ApplyFilterSortAndSearch(schools, queryFilter);
        int totalCount = await schools.CountAsync();
        schools = schools
            .OrderBy(y => y.SchoolName)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        var pagination = await schools.ToListAsync();
        return (pagination, totalCount);
    }

    private IQueryable<School> ApplyFilterSortAndSearch(IQueryable<School> schools, SchoolQueryFilter queryFilter)
    {
        schools = schools.Where(x => x.DeletedAt.Equals(null));
        
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            schools = schools.Where(x => x.SchoolName.ToLower().Contains(queryFilter.Search.ToLower()));
        }
        if (!string.IsNullOrEmpty(queryFilter.SearchProvince))
        {
            schools = schools.Where(x => x.Province.ProvinceName.ToLower().Contains(queryFilter.SearchProvince.ToLower()));
        }
        return schools;
    }
}