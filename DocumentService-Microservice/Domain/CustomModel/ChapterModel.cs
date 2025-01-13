namespace Domain.CustomModel;

public class ChapterModel
{
    public Guid? Id { get; set; } 
    public string? ChapterName { get; set; }
    public string? ChapterLevel { get; set; }
    public string? Description { get; set; }
    public string? Semester { get; set; }
    public int? NumberLesson { get; set; }
    public Guid? SubjectId { get; set; }
    public DateTime? CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
}