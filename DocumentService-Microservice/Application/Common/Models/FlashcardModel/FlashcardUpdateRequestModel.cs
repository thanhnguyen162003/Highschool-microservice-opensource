namespace Application.Common.Models.FlashcardModel;

public class FlashcardUpdateRequestModel
{
    public string? FlashcardName { get; set; }
    public Guid? SubjectId { get; set; }
    public string? FlashcardDescription { get; set; }
    public string? Status { get; set; }
}