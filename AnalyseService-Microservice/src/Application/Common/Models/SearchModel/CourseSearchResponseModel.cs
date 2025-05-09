namespace Application.Common.Models.SearchModel;

public class CourseSearchResponseModel
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Lesson? Lesson { get; set; }
    public SubjectCurriculum? SubjectCurriculum { get; set; }
    public Chapter? Chapter { get; set; }
    public Subject? Subject { get; set; }
}

public class Lesson
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
}

public class SubjectCurriculum
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
}

public class Chapter
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
}

public class Subject
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
}
