using Domain.CustomModel;
using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface ISubjectCurriculumRepository : IRepository<SubjectCurriculum>
{
    Task<bool> AddSubjectCurriculum(SubjectCurriculum subjectCurriculum, CancellationToken cancellationToken = default);
	Task<bool> UpdateSubjectCurriculum(SubjectCurriculum subjectCurriculum, CancellationToken cancellationToken = default);
    Task<bool> IsSubjectCurriculumPublish(Guid subjectId, Guid curriculumId, CancellationToken cancellationToken = default);
	Task<List<Curriculum>> GetCurriculumsOfSubject(Guid subjectId, CancellationToken cancellationToken = default);
	Task<List<SubjectCurriculum>> GetSubjectCurriculumOfCurriculum(Guid curriculumId, CancellationToken cancellationToken = default);
	Task<SubjectCurriculum> GetSubjectCurriculumById(Guid subjectCurriculumId, CancellationToken cancellationToken = default);
	Task<List<SubjectCurriculum>> GetSubjectCurriculaBySubjectId(Guid subjectId, CancellationToken cancellationToken = default);
	Task<bool> IsSubjectCurriculumExists(Guid subjectId, Guid curriculumId, CancellationToken cancellationToken = default);
    Task<SubjectCurriculum> GetSubjectCurriculum(Guid subjectId, Guid curriculumId, CancellationToken cancellationToken = default);
    Task<(List<SubjectCurriculum> subjectCurricula, int TotalCount)> GetSubjectCurriculaPublish(SubjectCurriculumQueryFilter queryFilter,
        CancellationToken cancellationToken = default);

    Task<List<SubjectCurriculum>> GetSubjectCurriculaRelated(Guid curriculumId,
        CancellationToken cancellationToken = default);
    Task<bool> UnPublishSubjectCurriculum(Guid id, CancellationToken cancellationToken = default);
    Task<bool> PublishSubjectCurriculum(Guid id, CancellationToken cancellationToken = default);
    Task<(List<SubjectCurriculum> subjectCurricula, int TotalCount)> GetSubjectCurriculaUnPublish(SubjectCurriculumQueryFilter queryFilter, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetSubjectCurriculumIdBySubjectId(IEnumerable<string> subjectIds);
    IEnumerable<CourseQueryModel> GetSubjectCurriculums();
    Task<IEnumerable<string>> CheckSubjectCurriculumName(IEnumerable<string> subjectCurriculumIds);
    Task<Dictionary<string, int>> GetSubjectCurriculumCount(CancellationToken cancellationToken = default);
}