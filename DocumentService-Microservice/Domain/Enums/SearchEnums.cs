

namespace Domain.Enums
{
    public enum IndexName
    {
        flashcard = 1,
        subject = 2,
        document = 3,
        name = 4,
        folder = 5,
        news = 6,
        course = 7,
    }

    public enum TypeEvent
    {
        Create = 1,
        Update = 2,
        Delete = 3
    }

    public enum SearchCourseType
    {
        Lesson = 1,
        Chapter = 2,
        SubjectCurriculum = 3,
        Subject = 4
    }

    public enum UpdateCourseType
    {
        lessonId = 1,
        chapterId = 2,
        subjectCurriculumId = 3,
        subjectId = 4
    }

}
