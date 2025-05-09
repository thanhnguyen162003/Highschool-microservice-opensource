using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class TheoryRepository(DocumentDbContext context) : BaseRepository<Theory>(context), ITheoryRepository
{
    public async Task<bool> SoftDelete(Guid theoryId, CancellationToken cancellationToken)
    {
        var theory = await _entities.Where(x => x.Id.Equals(theoryId)).FirstOrDefaultAsync(cancellationToken);
        if (theory == null)
        {
            return false;
        }
        theory.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CreateTheory(Theory theory, CancellationToken cancellationToken)
    {
        await _entities.AddAsync(theory, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> UpdateTheory(Theory theory, CancellationToken cancellationToken = default)
    {
        var theoryData = await _entities.FirstOrDefaultAsync(x => x.Id.Equals(theory.Id), cancellationToken);
        if (theoryData == null)
        {
            return false;
        }
        theoryData.TheoryName = theory.TheoryName ?? theoryData.TheoryName;
        theoryData.TheoryDescription = theory.TheoryDescription ?? theoryData.TheoryDescription;
        theoryData.TheoryContentHtml = theory.TheoryContentHtml ?? theoryData.TheoryContentHtml;
        theoryData.TheoryContentJson = theory.TheoryContentJson ?? theoryData.TheoryContentJson;
        theoryData.TheoryName = theory.TheoryName ?? theoryData.TheoryName;
        theoryData.UpdatedAt = DateTime.UtcNow;
        _entities.Update(theoryData);
        var result = await context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        return false;
    }
    public async Task<IEnumerable<Theory>> GetTheoryForTips(IEnumerable<string> theoryIds, CancellationToken cancellationToken = default)
    {
        var theory = await _entities
            .AsNoTracking()
            .Where(x => theoryIds.Contains(x.Id.ToString()))
            .Select(x => new Theory { Id = x.Id, TheoryName = x.TheoryName})
            .ToListAsync();
        return theory;
    }
    public async Task<(List<Theory> Theories, int TotalCount)> GetTheoryByFilters(Guid? lessonId, TheoryQueryFilter queryFilter, CancellationToken cancellationToken)
    {
        var theories = _entities
            .Where(x => lessonId.HasValue && x.LessonId == lessonId)
            .AsNoTracking();
        theories = ApplyFilterSortAndSearch(theories, queryFilter);
        int totalCount = await theories.CountAsync(cancellationToken);
        theories = theories.OrderBy(y => y)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        var pagnation = await theories.ToListAsync(cancellationToken);
        return (pagnation, totalCount);
    }

    public async Task<bool> TheoryIdExistAsync(Guid theoryId, CancellationToken cancellationToken)
    {
        return await _entities.AnyAsync(x => x.Id == theoryId, cancellationToken);
    }

    public async Task<Theory> GetTheoryByIdAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        return await _entities.Where(x=>x.Id.Equals(theoryId)).FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<Theory> ApplyFilterSortAndSearch(IQueryable<Theory> theories, TheoryQueryFilter queryFilter)
    {
        theories = theories.Where(x => x.DeletedAt.Equals(null));
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            theories = theories.Where(x => x.TheoryName.ToLower().Contains(queryFilter.Search.ToLower()));
        }
        return theories;
    }
}