namespace Domain.Entities;

public class RecommendedData
{
    public Guid Id { get; set; }
    public string? ObjectId { get; set; }
    public string? SubjectIds { get; set; }
    public string? DocumentIds { get; set; }
    public string? FlashcardIds { get; set; }
    public Guid? UserId { get; set; }
    public string? Grade { get; set; }
    public string? TypeExam { get; set; }
}