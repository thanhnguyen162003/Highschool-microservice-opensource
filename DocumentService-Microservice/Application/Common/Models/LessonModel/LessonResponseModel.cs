using Application.Common.Models.TheoryModel;

namespace Application.Common.Models.LessonModel;

public class LessonResponseModel
{
    public Guid Id { get; set; }
    public string? LessonName { get; set; }
    public Guid? ChapterId { get; set; }
    public string Slug { get; set; }
    public int? Like { get; set; }
    public string? VideoUrl { get; set; }
	public string? YoutubeVideoUrl { get; set; }
	public DateTime? CreatedAt { get; set; }
    public int? TheoryCount { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? IsDone { get; set; }
    public bool? IsCurrentView { get; set; }
}