using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.CustomModel;
using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces
{
	public interface IUserFlashcardProgressRepository : IRepository<UserFlashcardProgress>
	{
		Task<UserFlashcardProgress> GetProgressByUserAndContent(Guid userId, Guid flashcardContentId);
		Task<List<UserFlashcardProgress>> GetProgressByUser(Guid userId, Guid flashcardId);
		Task<bool> ResetProgressByUserProgressId(Guid id);
		Task<bool> AddAsync(UserFlashcardProgress progress);
		Task<bool> UpdateAsync(UserFlashcardProgress progress);
		Task<List<UserFlashcardProgress>> GetAllProgressByUser(Guid userId);
		Task<List<UserFlashcardLearningModel>> GetAllProgressLearning();
		Task<List<TopEnrolledSubjectModel>> GetEnrollmentCompletionStatus();
		Task<int> GetEnrollAmount();
        Task<bool> ResetFlashcardContentProgress(Guid userId, Guid flashcardContentId);

    }
}
