namespace Application.Common.Models.LessonModel;

public class LessonUpdateRequestModel
{
    public Guid LessonId { get; set; }
    public string? LessonName { get; set; }
	public string? YoutubeVideoUrl { get; set; }
	public string? LessonMaterial { get; set; }
    public int? DisplayOrder { get; set; }
}