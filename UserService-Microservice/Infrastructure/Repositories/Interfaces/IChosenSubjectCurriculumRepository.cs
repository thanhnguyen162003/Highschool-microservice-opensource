
namespace Infrastructure.Repositories.Interfaces
{
    public interface IChosenSubjectCurriculumRepository : IRepository<ChosenSubjectCurriculum>
	{
        Task<ChosenSubjectCurriculum> GetChosenByUserIdAndSubjectId(Guid userId, Guid subjectId, CancellationToken cancellationToken = default);
		Task<bool> InsertChosenSubjectCurriculum(ChosenSubjectCurriculum chosenSubjectCurriculum, CancellationToken cancellationToken = default);
		Task<bool> UpdateChosenSubjectCurriculum(ChosenSubjectCurriculum chosenSubjectCurriculum, CancellationToken cancellationToken = default);
	}
}
