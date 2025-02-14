namespace Application.KafkaMessageModel;

public class SubjectUpdateMessage
{
    public Guid SubjectId { get; set; }
    public string ImageRaw { get; set; }
}