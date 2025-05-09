namespace Application.Common.Models.FlashcardTestModel;

public class FlashcardContentTestQuestionModel
{
    public string Term { get; set; } = string.Empty;
    public Guid FlashcardContentId { get; set; }
    public List<AnswerOption> Options { get; set; } = new List<AnswerOption>();
    public class AnswerOption
    {
        // public Guid Id { get; set; } 
        public string Definition { get; set; } = string.Empty; 
    }
}