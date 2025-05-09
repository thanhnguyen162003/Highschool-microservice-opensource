namespace Application.Common.Models.FlashcardTestModel;

public class FlashcardTrueFalseTestResultModel
{
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public double Percentage { get; set; }
    public List<TrueFalseAnswerResultModel> AnswerResults { get; set; } = new();
}

public class TrueFalseAnswerResultModel
{
    public string Term { get; set; } = string.Empty;
    public Guid FlashcardContentId { get; set; }
    public bool SelectedOption { get; set; }
    public bool IsCorrect { get; set; }
}
