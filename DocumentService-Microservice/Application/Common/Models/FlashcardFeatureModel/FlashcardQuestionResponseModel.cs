namespace Application.Common.Models.FlashcardFeatureModel;

public class FlashcardQuestionResponseModel
{
    public bool NeedPayment { get; set; }
    public List<FlashcardContentQuestionModel> Questions { get; set; } = new List<FlashcardContentQuestionModel>();
}
