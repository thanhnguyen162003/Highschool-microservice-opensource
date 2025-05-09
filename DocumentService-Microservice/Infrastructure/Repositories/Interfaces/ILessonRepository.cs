using Domain.CustomModel;
using Domain.QueriesFilter;

namespace Infrastructure.Repositories.Interfaces;

public interface ILessonRepository : IRepository<Lesson>
{
    Task SoftDelete(IEnumerable<Guid> guids);
    Task<Lesson?> GetById(Guid lessonId);
    Task<(List<Lesson> Lessons, int TotalCount)> GetLessonsByFilters(Guid? chapterId, LessonQueryFilter queryFilter);
    Task<bool> UpdateLesson(Lesson lesson);
    Task<bool> CreateListLessons(List<Lesson> lessons);
    Task<bool> UpdateListLessons(List<Lesson?> lessons);
    Task<bool> LessonIdExistAsync(Guid? lessonId);
    Task<bool> AddSingleLesson(Lesson lesson);
    Task<IEnumerable<string>> GetLessonBySubjectId(IEnumerable<string> subjectIds, CancellationToken cancellationToken = default);
    IEnumerable<CourseQueryModel> GetLessons();
    Task<int> GetLessonsCount(CancellationToken cancellationToken = default);
}