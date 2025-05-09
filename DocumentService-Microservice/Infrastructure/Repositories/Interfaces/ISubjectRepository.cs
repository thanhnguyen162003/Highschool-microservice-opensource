using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<(List<Subject> Subjects, int TotalCount)> GetSubjects(SubjectQueryFilter queryFilter,
        CancellationToken cancellationToken = default);
    Task<SubjectModel> GetSubjectBySubjectId(Guid id, CancellationToken cancellationToken = default);
    Task<List<Subject>> GetAllSubjects(CancellationToken cancellationToken = default);
	Task<SubjectModel> GetSubjectBySubjectSlug(string slug, CancellationToken cancellationToken = default);
    Task<SubjectModel> GetSubjectBySubjectSlugAndCurriculum(string slug, Guid curriculumId, CancellationToken cancellationToken = default);
    Task<bool> DeleteSubject(Guid id, CancellationToken cancellationToken = default);
    Task<List<Subject>> GetAdditionalSubjects(string className, int count, CancellationToken cancellationToken);
    Task<List<Subject>> GetPlaceHolderSubjects();
    Task UpdateViewCount(Guid id);
    Task<bool> UpdateSubject(Subject dto, CancellationToken cancellationToken = default);
    Task<bool> CreateSubject(Subject dto, CancellationToken cancellationToken = default);
    Task<List<Subject>> GetSubjectsRelatedClass(string? className, CancellationToken cancellationToken = default);
    //Check Validation
    Task<bool> SubjectIdExistAsync(Guid? guid);
    Task<bool> SubjectNameExistAsync(string name);
    Task<IEnumerable<string>> CheckSubjectName(IEnumerable<string> subjectIds);
    Task<List<Subject>> GetSubjects(CancellationToken cancellationToken = default);
    Task<List<string>> GetSubjectIdsAsString(CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetGrade(List<string> subjectId, CancellationToken cancellationToken = default);
    IEnumerable<CourseQueryModel> GetSubjectsSearch();

    //recommended
    Task<List<Subject>> GetSubjectsByMasterSubjectId(Guid masterSubjectId, int? grade, CancellationToken cancellationToken);
    Task<List<Subject>> GetAdditionalSubjectsByCategory(int count, int? grade, CancellationToken cancellationToken);
    Task<List<Subject>> GetPlaceHolderSubjects(int? grade = null);
}