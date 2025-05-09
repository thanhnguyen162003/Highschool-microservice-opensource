namespace Domain.CustomModel;

public class ChapterSubjectModel
{
    public Guid? Id { get; set; }
    public string? ChapterName { get; set; }
    public string? ChapterLevel { get; set; }
    public Guid? SubjectId { get; set; } 
    public DateTime? CreatedAt { get; set; } 
}