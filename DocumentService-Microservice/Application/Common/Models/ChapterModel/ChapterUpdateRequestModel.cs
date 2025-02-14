namespace Application.Common.Models.ChapterModel;

public class ChapterUpdateRequestModel
{
    public Guid Id { get; set; }
    public string? ChapterName { get; set; }
    public int? ChapterLevel { get; set; }
    public string? Description { get; set; }
    public int? Semester { get; set; }
}