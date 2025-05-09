using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class ProvinceRepository(DocumentDbContext context) : BaseRepository<Province>(context), IProvinceRepository
{
    public async Task<bool> CreateProvinceAsync(Province province)
    {
        await _entities.AddAsync(province);
        var result = await context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteProvince(Guid id)
    {
        var doet = await _entities.FindAsync(id);
        if (doet is null)
        {
            return false;
        }
        doet.DeletedAt = DateTime.UtcNow;
        _entities.Update(doet);
        var result = await context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<(List<Province> Provinces, int TotalCount)> GetProvinceAsync(ProvinceQueryFilter queryFilter)
    {
        var provinces = _entities
            .AsNoTracking()
            .Include(x => x.Schools.Where(m => m.DeletedAt == null))
            .AsQueryable();

        // Apply filter, sort, and search without pagination to get the total count
        provinces = ApplyFilterSortAndSearch(provinces, queryFilter);
        int totalCount = await provinces.CountAsync(); // Get total count of filtered data

        // Apply pagination now
        provinces = provinces
            .OrderBy(y => y.Id)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        var paginatedProvinces = await provinces.ToListAsync();
        return (paginatedProvinces, totalCount); // Return both paginated data and total count
    }

    

    private IQueryable<Province> ApplyFilterSortAndSearch(IQueryable<Province> doets, ProvinceQueryFilter queryFilter)
    {
        doets = doets.Where(x => x.DeletedAt.Equals(null));
        
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            doets = doets.Where(x => x.ProvinceName.ToLower().Contains(queryFilter.Search.ToLower()));
        }
        return doets;
    }
}