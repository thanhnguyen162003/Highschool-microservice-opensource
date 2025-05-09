namespace Application.KafkaMessageModel;

public class TheoryCreatedFileModel
{
    public Guid TheoryId { get; set; }
    public string? FileUrl { get; set; }
    public string? PublicId { get; set; }
    public string? FileType { get; set; }
    public string? FileExtention { get; set; }
}