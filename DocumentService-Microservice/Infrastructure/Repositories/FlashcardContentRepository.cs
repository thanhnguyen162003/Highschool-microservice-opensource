using System.Linq.Expressions;
using Domain.Entities;
using Domain.QueriesFilter;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class FlashcardContentRepository(DocumentDbContext context) : BaseRepository<FlashcardContent>(context), IFlashcardContentRepository
{
    private readonly DocumentDbContext _context = context;

    public async Task<List<FlashcardContent>> GetFlashcardContent(FlashcardQueryFilter queryFilter, Guid flashcardId)
    {
        var flashcardContents = _entities.AsNoTracking().Where(x =>x.FlashcardId.Equals(flashcardId))
            .AsQueryable();
        flashcardContents = ApplyFilterSortAndSearch(flashcardContents, queryFilter);
        flashcardContents = flashcardContents.OrderBy(y => y.Rank)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        return await flashcardContents.ToListAsync();
    }

    public async Task<List<FlashcardContent>> GetFlashcardContentSort(Guid flashcardId, Guid userId)
    {
        var flashcardContents = _entities.AsNoTracking()
        .Where(x => x.FlashcardId.Equals(flashcardId))
        .Include(x => x.StudiableTerm)
        .AsQueryable();

        var nullStudiableTerm = flashcardContents.Where(x => x.StudiableTerm.Count == 0);
        if (await flashcardContents.AnyAsync(x => x.StudiableTerm.Count > 0))
        {
            flashcardContents = flashcardContents.Where(x => x.StudiableTerm.Any(st => st.Mode != "Learn" && st.UserId == userId));
        }
        return await flashcardContents.Union(nullStudiableTerm).ToListAsync();

    }
    public async Task<List<FlashcardContent>> GetFlashcardContentStarred(FlashcardQueryFilter queryFilter, Guid flashcardId, Guid userId)
    {
        var flashcardContents = _entities.AsNoTracking().Where(x => x.FlashcardId.Equals(flashcardId))
            .Include(x => x.StarredTerm)
            .Include(x => x.StudiableTerm)
            .AsQueryable();

        var studiableTerm = flashcardContents
            .Where(x => x.StudiableTerm.Any(st => st.UserId == userId)).Select(x => x.Id);

        var starredTerm = flashcardContents
            .Where(x => x.StarredTerm.Any(st => st.UserId == userId)).Select(x => x.Id);

        var nullTerm = flashcardContents.Where(x => !x.StarredTerm.Any(st => st.UserId == userId) && !x.StudiableTerm.Any(st => st.UserId == userId));

        var duplicateTermIds = studiableTerm.Intersect(starredTerm);
        var differentTermIds = studiableTerm.Except(starredTerm).Concat(studiableTerm.Except(starredTerm));

        var combinedTermIds = nullTerm.Select(x => x.Id)
            .Union(differentTermIds)
            .Union(duplicateTermIds)
            .Union(starredTerm);

        flashcardContents = _entities.AsNoTracking()
            .Where(x => x.FlashcardId.Equals(flashcardId))
            .Include(x => x.StarredTerm)
            .Include(x => x.StudiableTerm)
            .Where(x => combinedTermIds.Contains(x.Id));

        flashcardContents = ApplyFilterSortAndSearch(flashcardContents, queryFilter);
        flashcardContents = flashcardContents.OrderBy(y => y.Rank)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        return await flashcardContents.ToListAsync();
    }
    public async Task<List<FlashcardContent>> GetFlashcardContentByIds(List<Guid> flashcardContentIds)
    {
        return await _entities.AsNoTracking().Where(x => flashcardContentIds.Contains(x.Id)).ToListAsync();
    }

    public async Task<FlashcardContent> GetFlashcardContentByRank(int rank, CancellationToken cancellationToken)
    {
        return (await _entities.AsNoTracking().Where(x => x.Rank == rank).FirstOrDefaultAsync(cancellationToken))!;
    }

    public async Task<FlashcardContent> GetFlashcardContentById(Guid flashcarContentId)
    {
        var data =  await _entities.AsNoTracking().Where(x => x.Id.Equals(flashcarContentId)).FirstOrDefaultAsync();
        return data!;
    }

    public void UpdateRank(FlashcardContent flashcardContent)
    {
        _entities.Update(flashcardContent);
    }

    public async Task<IEnumerable<FlashcardContent>> GetAllAsync(Expression<Func<FlashcardContent, bool>> predicate)
    {
        return await _context.Set<FlashcardContent>().Where(predicate).ToListAsync();
    }

    public async Task<List<FlashcardContent>> GetFlashcardContentByIds(IEnumerable<Guid?> listId)
    {
        return await _entities.Where(x => listId.Any(id => id == x.Id)).ToListAsync();
    }

    public async Task<bool> CreateFlashcardContent(List<FlashcardContent> flashcardContents)
    {
        try
        {
            await _context.FlashcardContents.AddRangeAsync(flashcardContents);
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

    public async Task<bool> CreateFlashcardContentSingle(FlashcardContent flashcardContent)
    {
        try
        {
            await _context.FlashcardContents.AddAsync(flashcardContent);
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

    public async Task<bool> UpdateFlashcardContent(FlashcardContent flashcard, Guid id)
    {
        var flashcardUpdate = await _entities.FirstOrDefaultAsync(x => x.Flashcard.UserId.Equals(id) && x.Id.Equals(flashcard.Id));
        if (flashcardUpdate == null)
        {
            return false;
        }
        flashcardUpdate.FlashcardContentDefinition = flashcard.FlashcardContentDefinition ?? flashcardUpdate.FlashcardContentDefinition;
        flashcardUpdate.FlashcardContentTerm = flashcard.FlashcardContentTerm ?? flashcardUpdate.FlashcardContentTerm;
        flashcardUpdate.FlashcardContentDefinitionRichText = flashcard.FlashcardContentDefinitionRichText ?? flashcardUpdate.FlashcardContentDefinitionRichText;
        flashcardUpdate.FlashcardContentTermRichText = flashcard.FlashcardContentTermRichText ?? flashcardUpdate.FlashcardContentTermRichText;
        flashcardUpdate.Image = flashcard.Image ?? flashcardUpdate.Image;
        flashcardUpdate.Status = flashcard.Status ?? flashcardUpdate.Status;
        //flashcardUpdate.Rank = flashcard.Rank ?? flashcardUpdate.Rank;
        flashcardUpdate.UpdatedAt = DateTime.UtcNow;
        flashcardUpdate.UpdatedBy = flashcardUpdate.UpdatedBy;
        _entities.Update(flashcardUpdate);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> UpdateListFlashcardContent(List<FlashcardContent?> flashcard)
    {
        try
        {
             _context.FlashcardContents.UpdateRange(flashcard);
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

    public async Task<bool> DeleteFlashcardContent(Guid flashcardContentId, Guid userId)
    {
        var flashcards = await _entities.Where(x => x.Id.Equals(flashcardContentId) && x.Flashcard.UserId.Equals(userId)).FirstOrDefaultAsync();
        if (flashcards is null)
        {
            return false;
        }
        _entities.Remove(flashcards);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

	public async Task<List<FlashcardContent>> GetFlashcardContentByFlashcardId(Guid flashcardId)
	{
		var flashcardContents = await _entities.AsNoTracking().Where(x => x.FlashcardId.Equals(flashcardId) && x.DeletedAt == null)
			.ToListAsync();
        return flashcardContents;
	}

	private IQueryable<FlashcardContent> ApplyFilterSortAndSearch(IQueryable<FlashcardContent> flashcards, FlashcardQueryFilter queryFilter)
    {
        flashcards = flashcards.Where(x => x.DeletedAt == null).OrderBy(x=>x.Rank);
        
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            flashcards = flashcards.Where(x => x.FlashcardContentTerm.Contains(queryFilter.Search));
        }
        return flashcards;
    }
	public async Task<List<FlashcardContent>> GetFlashcardsWithinRankRange(int minRank, int maxRank, CancellationToken cancellationToken)
	{
		return await _context.FlashcardContents
			.Where(fc => fc.Rank >= minRank && fc.Rank <= maxRank)
			.OrderBy(fc => fc.Rank)
			.ToListAsync(cancellationToken);
	}
    
}