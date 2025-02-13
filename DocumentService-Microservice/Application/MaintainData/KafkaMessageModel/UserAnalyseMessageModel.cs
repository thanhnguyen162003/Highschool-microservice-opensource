namespace Application.KafkaMessageModel;

public class UserAnalyseMessageModel
{
    public Guid? UserId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? LessonId { get; set; }
    public Guid? FlashcardId { get; set; }
    public Guid? DocumentId { get; set; }
}