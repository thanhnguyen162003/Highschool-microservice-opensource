using Domain.QueriesFilter;
using Infrastructure.Constraints;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using System.ComponentModel;

namespace Infrastructure.Repositories;

public class FlashcardRepository(DocumentDbContext context) : BaseRepository<Flashcard>(context), IFlashcardRepository
{
    private readonly DocumentDbContext _context = context;

   public async Task<IEnumerable<Flashcard>> GetFlashcards()
    {
        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.DeletedAt == null && x.Created == true)
            .Include(y => y.FlashcardContents)
            .ToListAsync();

        foreach (var flashcard in flashcards)
        {
            if (flashcard.SubjectId != Guid.Empty)
            {
                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
                if (subjectExists)
                {
                    await _context.Entry(flashcard)
                        .Reference(f => f.Subject)
                        .Query()
                        .Include(s => s.Category)
                        .LoadAsync();
                }
            }
        }

        return flashcards;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcards(FlashcardQueryFilter queryFilter)
    {
        var flashcards = _entities.AsNoTracking()
            .Include(f => f.FlashcardContents)
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true)
            .AsQueryable();

        flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);
        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        var flashcardList = await flashcards.ToListAsync();

        foreach (var flashcard in flashcardList)
        {
            if (flashcard.SubjectId != Guid.Empty)
            {
                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
                if (subjectExists)
                {
                    await _context.Entry(flashcard)
                        .Reference(f => f.Subject)
                        .Query()
                        .Include(s => s.Category) // Ensure category is included
                        .LoadAsync();
                }
            }
        }

        return flashcardList;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsPlaceholder()
    {
        var flashcards = await _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true && x.DeletedAt == null)
            .OrderBy(x => x.CreatedAt)
            .Take(8)
            .Include(y => y.FlashcardContents)
            .ToListAsync();

        foreach (var flashcard in flashcards)
        {
            if (flashcard.SubjectId != Guid.Empty)
            {
                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
                if (subjectExists)
                {
                    await _context.Entry(flashcard)
                        .Reference(f => f.Subject)
                        .Query()
                        .Include(s => s.Category)
                        .LoadAsync();
                }
            }
        }

        return flashcards;
    }

    public async Task<IEnumerable<Flashcard>> GetTopFlashcard()
    {
        var flashcards = await _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true && x.DeletedAt == null)
            .OrderBy(x => x.Vote)
            .ThenBy(x => x.TodayView)
            .ThenBy(x => x.TotalView)
            .Take(8)
            .Include(y => y.FlashcardContents)
            .ToListAsync();

        foreach (var flashcard in flashcards)
        {
            if (flashcard.SubjectId != Guid.Empty)
            {
                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
                if (subjectExists)
                {
                    await _context.Entry(flashcard)
                        .Reference(f => f.Subject)
                        .Query()
                        .Include(s => s.Category)
                        .LoadAsync();
                }
            }
        }

        return flashcards;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsWithToken(FlashcardQueryFilter queryFilter, Guid userId)
    {
        var flashcards = _entities.AsNoTracking()
            .Where(x => x.Status.Equals(StatusConstrains.OPEN) || x.UserId.Equals(userId) && x.Created == true)
            .Include(y => y.FlashcardContents)
            .AsQueryable();

        flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);
        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        var flashcardList = await flashcards.ToListAsync();

        foreach (var flashcard in flashcardList)
        {
            if (flashcard.SubjectId != Guid.Empty)
            {
                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
                if (subjectExists)
                {
                    await _context.Entry(flashcard)
                        .Reference(f => f.Subject)
                        .Query()
                        .Include(s => s.Category)
                        .LoadAsync();
                }
            }
        }

        return flashcardList;
    }

    public async Task<IEnumerable<Flashcard>> GetOwnFlashcard(FlashcardQueryFilter queryFilter, Guid userId)
    {
        var flashcards = _entities.AsNoTracking()
            .Where(x => x.UserId.Equals(userId) && x.DeletedAt.Equals(null))
            .Include(y => y.FlashcardContents)
            .AsQueryable();

        flashcards = ApplyFilterSortAndSearchOwn(flashcards, queryFilter);
        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);

        var flashcardList = await flashcards.ToListAsync();

        foreach (var flashcard in flashcardList)
        {
            if (flashcard.SubjectId != Guid.Empty)
            {
                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
                if (subjectExists)
                {
                    await _context.Entry(flashcard)
                        .Reference(f => f.Subject)
                        .Query()
                        .Include(s => s.Category)
                        .LoadAsync();
                }
            }
        }

        return flashcardList;
    }

    public async Task<Flashcard> GetFlashcardById(Guid flashcardId)
    {
        var query = _entities
            .AsNoTracking()
            .Where(x =>
                x.Id.Equals(flashcardId) &&
                x.DeletedAt == null &&
                x.Created == true
            )
            .Include(y => y.FlashcardContents);

        var flashcard = await query.FirstOrDefaultAsync();

        if (flashcard != null && flashcard.SubjectId != Guid.Empty)
        {
            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
            if (subjectExists)
            {
                await _context.Entry(flashcard)
                    .Reference(f => f.Subject)
                    .Query()
                    .Include(s => s.Category)
                    .LoadAsync();
            }
        }

        return flashcard!;
    }
    public async Task<Flashcard> GetFlashcardByIdNoStatus(Guid flashcardId)
    {
        return (await _entities.AsNoTracking().Where(x => x.Id.Equals(flashcardId) && x.DeletedAt == null)
            .Include(y => y.FlashcardContents)
            .FirstOrDefaultAsync())!;
    }

    public async Task<Flashcard> GetFlashcardByIdWithToken(Guid flashcardId, Guid? userId)
    {
        var query = _entities
            .AsNoTracking()
            .Where(x =>
                x.Id.Equals(flashcardId) &&
                x.DeletedAt == null &&
                (
                    x.Created == true ||
                    (userId != Guid.Empty && x.Created == false && x.UserId == userId)
                )
            )
            .Include(y => y.FlashcardContents);

        var flashcard = await query.FirstOrDefaultAsync();

        if (flashcard != null && flashcard.SubjectId != Guid.Empty)
        {
            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
            if (subjectExists)
            {
                await _context.Entry(flashcard)
                    .Reference(f => f.Subject)
                    .Query()
                    .Include(s => s.Category)
                    .LoadAsync();
            }
        }

        return flashcard!;
    }

    public async Task<List<Flashcard>> GetFlashcardBySubjectIdFilter(List<Guid> subjectIds)
    {
        // Query to get flashcards where SubjectId is in the list of provided subjectIds
        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => subjectIds.Contains(x.SubjectId) && x.DeletedAt == null && x.Status.Equals(StatusConstrains.OPEN) && x.Created == true) 
            .Include(y => y.FlashcardContents)
            .Take(8)
            .ToListAsync(); 
        return flashcards;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsByUserId(FlashcardQueryFilter queryFilter, Guid userId)
    {
        var flashcards = _entities.AsNoTracking().Where(x=> x.Status.Equals(StatusConstrains.OPEN)
                                                            && x.UserId.Equals(userId) && x.Created == true)
            .Include(y=> y.FlashcardContents)
            .Include(s=>s.Subject!)
            .ThenInclude(c=>c.Category)
            .AsQueryable();
        flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);
        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        return await flashcards.ToListAsync();
    }

    public Task<int> CheckNumberFlashcardInUser(Guid userId)
    {
        var flashcardsNumber = _entities.AsNoTracking().Where(x => x.UserId.Equals(userId)).CountAsync();
        return flashcardsNumber;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardByUsername(FlashcardQueryFilter queryFilter, string username)
    {
        throw new NotImplementedException();
        // var flashcards = _entities.AsNoTracking().Where(x=> x.Status.Equals(StatusConstrains.OPEN)
        //                                                     && x.UserId.Equals(userId))
        //     .Include(y=> y.FlashcardContents)
        //     .AsQueryable();
        // flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);
        // flashcards = flashcards.OrderBy(y => y.CreatedAt)
        //     .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
        //     .Take(queryFilter.PageSize);
        // return await flashcards.ToListAsync();
    }

    public async Task<Flashcard> GetFlashcardDraftById(Guid flashcardId, Guid userId)
    {
        return (await _entities.AsNoTracking().Where(x => x.Id.Equals(flashcardId) && x.Created == false && x.UserId.Equals(userId))
            .Include(y=> y.FlashcardContents)
            .FirstOrDefaultAsync())!;
    }

    public async Task<Flashcard> GetFlashcardByUserId(Guid userId)
    {
        return await _entities.AsNoTracking().Where(x => x.UserId.Equals(userId) && x.DeletedAt == null && x.Created == true).FirstOrDefaultAsync();
    }
    public async Task<Flashcard> GetFlashcardDraftByUserId(Guid userId)
    {
        return (await _entities.AsNoTracking().Where(x => x.UserId.Equals(userId) && x.Created == false).FirstOrDefaultAsync())!;
    }

    public async Task<Flashcard> GetFlashcardBySlug(string slug, Guid? userId)
    {
        var query = _entities.AsNoTracking()
            .Where(x =>
                x.Slug.Equals(slug) &&
                (x.Status.Equals(StatusConstrains.OPEN) || x.Status.Equals(StatusConstrains.ONLYLINK)) &&
                x.DeletedAt == null &&
                (
                    x.Created == true || 
                    (userId != Guid.Empty && x.Created == false && x.UserId == userId)
                )
            )
            .Include(f => f.FlashcardContents);

        var flashcard = await query.FirstOrDefaultAsync();

        if (flashcard != null && flashcard.SubjectId != Guid.Empty)
        {
            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
            if (subjectExists)
            {
                await _context.Entry(flashcard)
                    .Reference(f => f.Subject)
                    .Query()
                    .Include(s => s.Category)
                    .LoadAsync();
            }
        }

        return flashcard!;
    }
    
    public async Task<bool> DeleteFlashcard(Guid flashcardId, Guid userId)
    {
        var flashcards = await _entities.Where(x => x.Id.Equals(flashcardId) && x.UserId.Equals(userId) && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (flashcards is null)
        {
            return false;
        }
        flashcards.DeletedAt = DateTime.Now;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<Flashcard> GetFlashcardDetail(Guid flashcardId)
    {
        return (await _entities.AsNoTracking()
            .Include(y=> y.FlashcardContents)
            .FirstOrDefaultAsync())!;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsBySubject(FlashcardQueryFilter queryFilter, Guid id)
    {
        var flashcards = _entities.Where(x=>x.SubjectId.Equals(id) &&
                                            (x.Status.Equals(StatusConstrains.OPEN))).AsQueryable();
        flashcards = ApplyFilterSortAndSearch(flashcards, queryFilter);
        flashcards = flashcards.OrderBy(y => y.CreatedAt)
            .Skip((queryFilter.PageNumber - 1) * queryFilter.PageSize)
            .Take(queryFilter.PageSize);
        return await flashcards.ToListAsync();
    }

    public async Task<bool> CreateFlashcard(Flashcard flashcard)
    {
        _context.Flashcards.Add(flashcard);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateFlashcard(Flashcard flashcard, Guid id)
    {
        var flashcardUpdate = await _entities.Where(x => x.Id.Equals(flashcard.Id) && x.UserId.Equals(id) && x.DeletedAt == null).FirstOrDefaultAsync();

        if (flashcardUpdate == null)
        {
            return false;
        }
        flashcardUpdate.FlashcardName = flashcard.FlashcardName ?? flashcardUpdate.FlashcardName;
        flashcardUpdate.SubjectId = flashcard.SubjectId;
        flashcardUpdate.FlashcardDescription = flashcard.FlashcardDescription ?? flashcardUpdate.FlashcardDescription;
        flashcardUpdate.Slug = flashcard.Slug ?? flashcardUpdate.Slug;
        flashcardUpdate.Status = flashcard.Status ?? flashcardUpdate.Status;
        flashcardUpdate.UpdatedAt = DateTime.Now;

        _entities.Update(flashcardUpdate);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }
    public async Task<bool> UpdateCreatedFlashcard(Flashcard flashcard, Guid id)
    {
        _entities.Update(flashcard);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }
    
    private IQueryable<Flashcard> ApplyFilterSortAndSearch(IQueryable<Flashcard> flashcards, FlashcardQueryFilter queryFilter)
    {
        flashcards = flashcards.Where(x => x.DeletedAt.Equals(null));
        
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            flashcards = flashcards.Where(x => x.FlashcardName.ToLower().Contains(queryFilter.Search.ToLower()));
        }
        return flashcards;
    }
    private IQueryable<Flashcard> ApplyFilterSortAndSearchOwn(IQueryable<Flashcard> flashcards, FlashcardQueryFilter queryFilter)
    {
        flashcards = flashcards.Where(x => x.DeletedAt.Equals(null));
        if (!string.IsNullOrEmpty(queryFilter.Search))
        {
            flashcards = flashcards.Where(x => x.FlashcardName.Contains(queryFilter.Search));
        }
        return flashcards;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsBySubjectId(Guid subjectId, CancellationToken cancellationToken = default)
    {
        return await _entities.AsNoTracking()
            .Where(x => x.SubjectId.Equals(subjectId)&& x.Created == true)
            .Include(s=>s.Subject!)
            .ThenInclude(c=>c.Category!)
            .OrderByDescending(x => x.CreatedAt)
            .Take(4)
            .ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<string>> GetFlashcardBySubjectId(IEnumerable<string> subjectIds, CancellationToken cancellationToken = default)
    {
        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => subjectIds.Contains(x.SubjectId.ToString()))
            .Select(x => x.Id.ToString())
            .ToListAsync(cancellationToken);
        return (IEnumerable<string>)flashcards;
    }
    public async Task<IEnumerable<Flashcard>> GetFlashcardForTips(IEnumerable<string> flashcardIds, CancellationToken cancellationToken = default)
    {
        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => flashcardIds.Contains(x.Id.ToString()))
            .Select(x => new Flashcard { Id = x.Id, FlashcardName = x.FlashcardName, Slug = x.Slug})
            .ToListAsync();
        return flashcards;
    }
    public async Task<IEnumerable<Guid>> ExceptExistFlashcards(IEnumerable<Guid> flashcardIds)
    {
        var flashcards = await _entities
            .AsNoTracking()
            .Where(x => flashcardIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var result = flashcardIds.Except(flashcards).ToList();

        return result;
    }

	public async Task<IEnumerable<Flashcard>> EveryOneAlsoWatch()
	{
		var flashcards = await _entities.AsNoTracking()
			.Where(x => x.Status.Equals(StatusConstrains.OPEN) && x.Created == true && x.DeletedAt == null)
			.OrderBy(x => x.TodayView)
			.ThenBy(x => x.TotalView)
			.ThenBy(x => x.Vote)
			.Take(20)
			.Include(y => y.FlashcardContents)
			.ToListAsync();

		foreach (var flashcard in flashcards)
		{
			if (flashcard.SubjectId != Guid.Empty)
			{
				var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == flashcard.SubjectId);
				if (subjectExists)
				{
					await _context.Entry(flashcard)
						.Reference(f => f.Subject)
						.Query()
						.Include(s => s.Category)
						.LoadAsync();
				}
			}
		}

		return flashcards;
	}
}