using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Infrastructure.Repositories
{
    public class StudiableTermRepository(DocumentDbContext context) : BaseRepository<StudiableTerm>(context), IStudiableTermRepository
    {
        public async Task<bool> CreateStudiableTermList(List<StudiableTerm> studiableTerm, CancellationToken cancellation)
        {
            await _entities.AddRangeAsync(studiableTerm, cancellation);
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CreateStudiableTerm(StudiableTerm studiableTerm, CancellationToken cancellation)
        {
            await _entities.AddAsync(studiableTerm, cancellation);
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateStudiableTermList(List<StudiableTerm> studiableTerm, CancellationToken cancellation)
        {
            try
            {
                _entities.UpdateRange(studiableTerm);
                var result = await context.SaveChangesAsync(cancellation);

                return result > 0;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error updating StudiableTerms: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating StudiableTerms: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateStudiableTerm(StudiableTerm studiableTerm, CancellationToken cancellation)
        {
            try
            {
                _entities.Update(studiableTerm);
                var result = await context.SaveChangesAsync(cancellation);

                return result > 0;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error updating StudiableTerms: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating StudiableTerms: {ex.Message}");
                return false;
            }
        }

        public async Task<List<StudiableTerm>> GetStudiableTerm(Guid userId, Guid contentId)
        {
            var result = await _entities.Where(x => x.UserId.Equals(userId) && x.ContainerId.Equals(contentId)).ToListAsync();
            return result;
        }

        public async Task<bool> DeleteStudiableTerm(List<StudiableTerm> studiableTerms, CancellationToken cancellationToken)
        {
            _entities.RemoveRange(studiableTerms);
            var result = await context.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<StudiableTerm>> CheckDuplicateStudiableTerm(Guid userId, List<Guid> flashcardContentIds, Guid contentId)
        {
            var result = await _entities
            .Where(x => x.UserId.Equals(userId) && flashcardContentIds.Contains(x.FlashcardContentId.Value) && x.ContainerId.Equals(contentId))
            .ToListAsync();
            return result;
        }
    }
}
