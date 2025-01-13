namespace Application.Common.Models.FlashcardTestModel;

public class FlashcardAnswerSubmissionModel
{
    public Guid FlashcardContentId { get; set; } 
    public string SubmittedAnswer { get; set; } = string.Empty;
}