using Domain.Entities;

namespace Domain.CustomModel;

public class LessonModel
{
    public Guid Id { get; set; }
    public string? LessonName { get; set; }
    public string? LessonBody { get; set; }
    public string? Slug { get; set; }
    public string? LessonMaterial { get; set; }
    public int? Like { get; set; }
    public Guid? ChapterId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? DisplayOrder { get; set; }
    public int? TheoryCount { get; set; }
	public string? YoutubeVideoUrl { get; set; }
	public string? VideoUrl { get; set; }
    public bool? IsDone { get; set; }
    public bool? IsCurrentView { get; set; }
}