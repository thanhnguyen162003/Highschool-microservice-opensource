using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
    public class StarredTermRepository(DocumentDbContext context) : BaseRepository<StarredTerm>(context), IStarredTermRepository
    {
        public async Task<bool> AddStarredTerm(StarredTerm starredTerm)
        {
            await _entities.AddAsync(starredTerm);
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }


        public async Task<bool> DeleteStarredTerm(StarredTerm starredTerm)
        {
            var star = await _entities.Where(x => x.UserId.Equals(starredTerm.UserId) && x.FlashcardContentId.Equals(starredTerm.FlashcardContentId) && x.ContainerId.Equals(starredTerm.ContainerId)).SingleOrDefaultAsync();
            if (star != null)
            {
                _entities.Remove(star);
            }
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CheckDuplicateStarredTerm(Guid userId, Guid flashcardContentId, Guid containerId)
        {
            var result = await _entities.Where(x => x.UserId.Equals(userId) && x.FlashcardContentId.Equals(flashcardContentId) && x.ContainerId.Equals(containerId)).SingleOrDefaultAsync();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<List<StarredTerm>> GetStarredTerm(Guid userId, Guid contentId)
        {
            var result = await _entities.Where(x => x.UserId.Equals(userId) && x.ContainerId.Equals(contentId)).ToListAsync();
            return result;
        }

        //public async Task<List<Category>> GetCategories()
        //{
        //    return await _entities.ToListAsync();
        //}
    }
}
