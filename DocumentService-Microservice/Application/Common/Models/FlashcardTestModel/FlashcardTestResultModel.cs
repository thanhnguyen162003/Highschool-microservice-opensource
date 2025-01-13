namespace Application.Common.Models.FlashcardTestModel;

public class FlashcardTestResultModel
{
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; } 
    public double Percentage { get; set; }
    public List<FlashcardAnswerResultModel> AnswerResults { get; set; } = new List<FlashcardAnswerResultModel>(); 
}

public class FlashcardAnswerResultModel
{
    public string Term { get; set; } = string.Empty;
    public Guid FlashcardContentId { get; set; }
    public string SubmittedAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
}
