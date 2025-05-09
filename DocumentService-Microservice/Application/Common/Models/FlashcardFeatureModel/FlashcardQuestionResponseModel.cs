namespace Application.Common.Models.FlashcardFeatureModel;

public class FlashcardQuestionResponseModel
{
    public string Message { get; set; } = "Success";
    public List<FlashcardContentQuestionModel> Questions { get; set; } = new List<FlashcardContentQuestionModel>();
}
