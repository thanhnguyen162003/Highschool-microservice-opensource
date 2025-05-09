namespace Application.Common.Models.TheoryModel;

public class TheoryLessonResponseModel
{
    public Guid? Id { get; set; }
    
    public string? TheoryTitle { get; set; }
    
    public string? TheoryDescription { get; set; }

    public string? TheoryContentJson { get; set; }

    public string? TheoryContentHtml { get; set; }
}