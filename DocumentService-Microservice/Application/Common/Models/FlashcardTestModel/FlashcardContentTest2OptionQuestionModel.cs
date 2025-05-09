namespace Application.Common.Models.FlashcardTestModel;

public class FlashcardContentTest2OptionQuestionModel
{
    public string Term { get; set; } = string.Empty;
    public List<AnswerOption> Options { get; set; } = new List<AnswerOption>();
    public class AnswerOption
    {
        public Guid Id { get; set; } 
        public bool Option { get; set; } 
        
    }
}