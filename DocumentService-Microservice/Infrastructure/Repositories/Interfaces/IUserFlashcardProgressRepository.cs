using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
	public interface IUserFlashcardProgressRepository : IRepository<UserFlashcardProgress>
	{
		Task<UserFlashcardProgress> GetProgressByUserAndContent(Guid userId, Guid flashcardContentId);
		Task<List<UserFlashcardProgress>> GetProgressByUser(Guid userId, Guid flashcardId);
		Task<bool> ResetProgressByUserProgressId(Guid id);
		Task<bool> AddAsync(UserFlashcardProgress progress);
		Task<bool> UpdateAsync(UserFlashcardProgress progress);
	}
}
