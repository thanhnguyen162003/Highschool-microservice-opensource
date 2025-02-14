using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;


namespace Infrastructure.Repositories
{
	public class UserFlashcardProgressRepository(DocumentDbContext context) : BaseRepository<UserFlashcardProgress>(context), IUserFlashcardProgressRepository
	{
		public async Task<bool> ResetProgressByUserProgressId(Guid id)
		{
			var progress = await _entities.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
			_entities.Remove(progress);
			var result = await context.SaveChangesAsync();
			return result > 0;
		}

		public async Task<bool> AddAsync(UserFlashcardProgress progress)
		{
			await _entities.AddAsync(progress);
			var result = await context.SaveChangesAsync();
			return result > 0;
		}

		public async Task<UserFlashcardProgress> GetProgressByUserAndContent(Guid userId, Guid flashcardContentId)
		{
			return await _entities
				.FirstOrDefaultAsync(p => p.UserId == userId && p.FlashcardContentId == flashcardContentId);
		}

		public async Task<List<UserFlashcardProgress>> GetProgressByUser(Guid userId, Guid flashcardId)
		{
			return await _entities.AsNoTracking()
				.Where(p => p.UserId == userId && p.FlashcardId == flashcardId)
				.ToListAsync();
		}

		public async Task<bool> UpdateAsync(UserFlashcardProgress progress)
		{
			_entities.Update(progress);
			var result = await context.SaveChangesAsync();
			return result > 0;
		}
	}
}
