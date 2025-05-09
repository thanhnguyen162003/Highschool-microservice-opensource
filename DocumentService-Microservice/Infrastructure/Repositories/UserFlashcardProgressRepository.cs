using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.CustomModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

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
				.Where(p => p.UserId == userId && p.FlashcardContent.FlashcardId == flashcardId)
                .Include(p => p.FlashcardContent)
				.ToListAsync();
		}

		public async Task<bool> UpdateAsync(UserFlashcardProgress progress)
		{
			_entities.Update(progress);
			var result = await context.SaveChangesAsync();
			return result > 0;
		}
        public async Task<int> GetEnrollAmount()
        {
            var result = await _entities.GroupBy(x=>x.UserId).CountAsync();
            return result;
        }

        public async Task<List<UserFlashcardProgress>> GetAllProgressByUser(Guid userId)
		{
			return await _entities.AsNoTracking()
				.Where(p => p.UserId == userId)
				.Include(f => f.FlashcardContent)
				.ToListAsync();
		}

        public async Task<List<UserFlashcardLearningModel>> GetAllProgressLearning()
        {
            return await _entities.AsNoTracking()
                .Where(p => p.LastReviewDate.HasValue && p.LastReviewDate.Value.Date <= DateTime.UtcNow.Date)
                .Include(f => f.FlashcardContent)
                .Select(p => new UserFlashcardLearningModel()
                {
                    UserId = p.UserId,
                    FlashcardContentId = p.FlashcardContentId,
                    FlashcardId = p.FlashcardContent.FlashcardId,
                    LastReviewDateHistory = p.LastReviewDateHistory,
					TimeSpentHistory = p.TimeSpentHistory
                })
                .ToListAsync();
        }
        public async Task<List<TopEnrolledSubjectModel>> GetEnrollmentCompletionStatus()
        {
            // Get enrollment stats
            var enrollments = await _entities
                .AsNoTracking()
                .Include(e => e.FlashcardContent)
                .ThenInclude(fc => fc.Flashcard)
                .GroupBy(e => e.FlashcardContent.FlashcardId)
                .Select(g => new
                {
                    FlashcardId = g.Key,
                    Name = g.First().FlashcardContent.Flashcard.FlashcardName,
                    TotalEnrollmentCount = g.Select(p => p.UserId).Distinct().Count()
                })
                .OrderByDescending(s => s.TotalEnrollmentCount)
                .Take(5)
                .ToListAsync();

            // Get required flashcard content per flashcard
            var requiredLessons = await _entities
                .Where(e => enrollments.Select(en => en.FlashcardId).Contains(e.FlashcardContent.FlashcardId))
                .Select(e => new
                {
                    e.FlashcardContent.FlashcardId,
                    FlashcardContentId = e.FlashcardContent.Flashcard.FlashcardContents.Select(fc => fc.Id)
                })
                .ToListAsync();

            // Map required content for each flashcard
            var requiredLessonMap = requiredLessons
                .GroupBy(l => l.FlashcardId)
                .ToDictionary(g => g.Key, g => g.SelectMany(x => x.FlashcardContentId).ToHashSet());

            // Get user progress for each flashcard
            var requiredFlashcardIds = requiredLessonMap.Keys.ToList(); // Convert keys to a list

            var userProgress = await _entities
                .Where(e => requiredFlashcardIds.Contains(e.FlashcardContent.FlashcardId)) // Use .Contains() instead
                .Select(e => new
                {
                    e.UserId,
                    e.FlashcardContent.FlashcardId,
                    e.FlashcardContent.Id
                })
                .ToListAsync();

            // Group progress by user & flashcard
            var userCompletionMap = userProgress
                .GroupBy(p => new { p.UserId, p.FlashcardId })
                .ToDictionary(g => g.Key, g => g.Select(p => p.Id).ToHashSet());

            // Calculate completion percentage
            var topSubjects = enrollments.Select(subject =>
            {
                int completedUsers = userCompletionMap
                    .Where(kvp => kvp.Key.FlashcardId == subject.FlashcardId)
                    .Count(kvp => kvp.Value.IsSupersetOf(requiredLessonMap[subject.FlashcardId]));

                return new TopEnrolledSubjectModel
                {
                    Name = subject.Name,
                    TotalEnrollmentCount = subject.TotalEnrollmentCount,
                    Completion = (int)((completedUsers / (double)subject.TotalEnrollmentCount) * 100)
                };
            }).ToList();

            return topSubjects;
        }
        public async Task<bool> ResetFlashcardContentProgress(Guid userId, Guid flashcardContentId)
        {
            var progress = await _entities.AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.FlashcardContentId == flashcardContentId);

            if (progress == null)
            {
                return false;
            }

            _entities.Remove(progress);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}
