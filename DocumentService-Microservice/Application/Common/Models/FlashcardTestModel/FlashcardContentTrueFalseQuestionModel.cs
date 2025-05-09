namespace Application.Common.Models.FlashcardTestModel;

public class FlashcardContentTrueFalseQuestionModel
{
    public string Term { get; set; } = string.Empty;
    public Guid FlashcardContentId { get; set; }
    public string PresentedDefinition { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } 
}
