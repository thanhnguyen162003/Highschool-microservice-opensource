namespace Application.Common.Models.FlashcardTestModel;

public class TrueFalseAnswerSubmissionModel
{
    public Guid FlashcardContentId { get; set; }
    public bool SelectedOption { get; set; }
}