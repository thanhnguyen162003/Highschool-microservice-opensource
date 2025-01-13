namespace Application.Common.Models.LessonModel;

public class LessonCreateRequestModel
{
    public string? LessonName { get; set; }
    // public string? LessonBody { get; set; }
    public string? LessonMaterial { get; set; }
    public int? DisplayOrder { get; set; }
}