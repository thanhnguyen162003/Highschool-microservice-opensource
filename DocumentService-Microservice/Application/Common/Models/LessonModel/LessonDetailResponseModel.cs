using Application.Common.Models.TheoryModel;

namespace Application.Common.Models.LessonModel;

public class LessonDetailResponseModel
{
    public Guid Id { get; set; }
    public string? LessonName { get; set; }
    public string? LessonMaterial { get; set; }
    public string? YoutubeVideoUrl { get; set; }
    public string? Slug { get; set; }
    public int? Like { get; set; }
    public string? VideoUrl { get; set; }
    public Guid? ChapterId { get; set; }
    public int? TheoryCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public ICollection<TheoryLessonResponseModel>? Theories { get; set; }
    public Guid? NextLessonId { get; set; }
    public Guid? NextChapterId { get; set; }
    public int? DisplayOrder { get; set; }
}