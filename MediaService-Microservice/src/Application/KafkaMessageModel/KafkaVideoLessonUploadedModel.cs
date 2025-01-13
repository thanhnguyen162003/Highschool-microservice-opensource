namespace Application.KafkaMessageModel;

public class KafkaVideoLessonUploadedModel
{
    public string VideoUrl { get; set; }
    public Guid LessonId { get; set; }
}
