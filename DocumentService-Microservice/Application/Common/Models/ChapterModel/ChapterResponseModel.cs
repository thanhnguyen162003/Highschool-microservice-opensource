namespace Application.Common.Models.ChapterModel;

public class ChapterResponseModel
{
    public Guid? Id { get; set; } 
    public string? ChapterName { get; set; }
    public int? ChapterLevel { get; set; }
    public string? Description { get; set; }
    public Guid? SubjectCurriculumId { get; set; }
    public string? CurriculumName { get; set; }
    public int? Semester { get; set; }
    public int? NumberLesson { get; set; }
    public DateTime? CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
    public bool? IsDone { get; set; }
}