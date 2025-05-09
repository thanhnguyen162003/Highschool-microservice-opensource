
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
	public class ChosenSubjectCurriculumRepository(UserDatabaseContext context) : BaseRepository<ChosenSubjectCurriculum>(context), IChosenSubjectCurriculumRepository
	{
		public async Task<ChosenSubjectCurriculum> GetChosenByUserIdAndSubjectId(Guid userId, Guid subjectId, CancellationToken cancellationToken = default)
		{
			var result = await _entities.Where(x => x.UserId == userId && x.SubjectId == subjectId).FirstOrDefaultAsync();
			return result;
		}

		public async Task<bool> InsertChosenSubjectCurriculum(ChosenSubjectCurriculum chosenSubjectCurriculum, CancellationToken cancellationToken = default)
		{
			await _entities.AddAsync(chosenSubjectCurriculum);
			var result = await context.SaveChangesAsync(cancellationToken);
			return result > 0;
		}

		public async Task<bool> UpdateChosenSubjectCurriculum(ChosenSubjectCurriculum chosenSubjectCurriculum, CancellationToken cancellationToken = default)
		{
			_entities.Update(chosenSubjectCurriculum);
			var result = await context.SaveChangesAsync(cancellationToken);
			return result > 0;
		}
	}
}
