namespace Application.KafkaMessageModel;

public class SubjectImageCreatedModel
{
    public Guid SubjectId { get; set; }
    public string? ImageUrl { get; set; }
    public string? PublicIdUrl { get; set; }
    public string? Format { get; set; }
}