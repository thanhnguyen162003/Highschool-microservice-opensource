using Domain.CustomModel;
using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface IChapterRepository : IRepository<Chapter>
{
    Task<(List<Chapter> Chapters, int TotalCount)> GetChapters(ChapterQueryFilter queryFilter);
    Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersModerator(ChapterQueryFilter queryFilter);
    Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubject(ChapterQueryFilter queryFilter, Guid subjectId);
    Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumId(ChapterQueryFilter queryFilter, Guid subjectCurriculumId);
    Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumIdModerator(ChapterQueryFilter queryFilter, Guid subjectCurriculumId);
    Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculum(ChapterQueryFilter queryFilter, Guid curriculumId, Guid subjectId);
    Task<(List<Chapter> Chapters, int TotalCount)> GetChaptersBySubjectCurriculumModerator(ChapterQueryFilter queryFilter, Guid curriculumId, Guid subjectId);
    Task<ChapterModel> GetChapterByChapterId(Guid id);
    Task<bool> DeleteChapter(Guid id);
    Task<bool> UpdateChapter(Chapter chapter);
    Task<bool> CreateChapter(Chapter dto);
    Task<bool> CreateChapterList(List<Chapter> listChapters);
    // Validation
    Task<bool> ChapterIdExistsAsync(Guid? guid);
    Task<bool> ChapterNameExistsAsync(string name);
    IEnumerable<CourseQueryModel> GetChapters();
}