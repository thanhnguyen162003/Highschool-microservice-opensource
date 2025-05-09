using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
    public class FSRSPresetRepository(DocumentDbContext context) : BaseRepository<FSRSPreset>(context), IFSRSPresetRepository
    {
        private readonly DocumentDbContext _context = context;

        public async Task<FSRSPreset> GetPresetById(Guid id)
        {
            return await _entities.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }
        public async Task<(double[] FsrsParameters, double Retrievability)?> GetPresetParameterAndR(Guid id)
        {
            var result = await _entities
                .Where(x => x.Id == id)
                .Select(x => new { x.FsrsParameters, x.Retrievability })
                .FirstOrDefaultAsync();

            return result != null ? (result.FsrsParameters, result.Retrievability) : null;
        }
        public async Task<(List<FSRSPreset> Presets, int TotalCount)> GetPresetAsync(FSRSPresetQueryFilter queryFilter, Guid userId)
        {
            var defaultList = _entities
                .Where(x => x.IsPublicPreset)
                .AsNoTracking();

            var additionalList = _entities
                .Where(x => x.UserId == userId && !defaultList.Select(d => d.Id).Contains(x.Id))
                .AsNoTracking();

            var resultList = defaultList.Concat(additionalList);
            // Apply filter, sort, and search without pagination to get the total count
            resultList = ApplyFilterSortAndSearch(resultList, queryFilter);
            int totalCount = await resultList.CountAsync(); // Get total count of filtered data

            // Apply pagination now
            resultList = resultList
                .OrderBy(y => y.Id)
                .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
                .Take(queryFilter.PageSize);

            var paginatedProvinces = await resultList.ToListAsync();
            return (paginatedProvinces, totalCount); // Return both paginated data and total count
        }
        public async Task<(List<FSRSPreset> Presets, int TotalCount)> GetPresetAsyncAdmin(FSRSPresetQueryFilter queryFilter)
        {
            var defaultList = _entities
                .Where(x => x.IsPublicPreset)
                .AsNoTracking();

            var additionalList = _entities
                .Where(x => !defaultList.Select(d => d.Id).Contains(x.Id))
                .AsNoTracking();

            var resultList = defaultList.Concat(additionalList);
            // Apply filter, sort, and search without pagination to get the total count
            resultList = ApplyFilterSortAndSearch(resultList, queryFilter);
            int totalCount = await resultList.CountAsync(); // Get total count of filtered data

            // Apply pagination now
            resultList = resultList
                .OrderBy(y => y.Id)
                .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
                .Take(queryFilter.PageSize);

            var paginatedProvinces = await resultList.ToListAsync();
            return (paginatedProvinces, totalCount); // Return both paginated data and total count
        }
        public async Task<List<FSRSPreset>> GetPreset(Guid userId,CancellationToken cancellationToken = default)
        {
            var defaultList = await _entities.Where(x => x.IsPublicPreset == true).AsNoTracking().ToListAsync();
            var additionalList = await _entities.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
            defaultList.AddRange(additionalList);
            return defaultList;
        }
        public async Task<bool> DeletePreset(Guid id, CancellationToken cancellationToken = default)
        {
            var subject = await _entities.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
            if (subject == null)
            {
                return false;
            }
            _entities.Remove(subject);
            var result = await _context.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdatePreset(FSRSPreset fSRSPreset, CancellationToken cancellation)
        {
            try
            {
                _entities.Update(fSRSPreset);
                var result = await _context.SaveChangesAsync(cancellation);

                return result > 0;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error updating FSRS preset: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating FSRS preset: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> CreatePreset(FSRSPreset fSRSPreset, CancellationToken cancellationToken = default)
        {
            await _entities.AddAsync(fSRSPreset, cancellationToken);
            var result = await _context.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return true;
            }
            return false;
        }
        private IQueryable<FSRSPreset> ApplyFilterSortAndSearch(IQueryable<FSRSPreset> doets, FSRSPresetQueryFilter queryFilter)
        {
            if (!string.IsNullOrEmpty(queryFilter.Search))
            {
                doets = doets.Where(x => x.Title.ToLower().Contains(queryFilter.Search.ToLower()));
            }
            return doets;
        }
    }
    
} 