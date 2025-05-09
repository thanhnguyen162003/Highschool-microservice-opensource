using Domain.CustomModel;

namespace Application.Common.Models.FlashcardModel
{
    public class FlashcardStudyResponseModel : Domain.CustomModel.FlashcardModel
    {
        public int NewCardCount { get; set; }
        public int CardInLearningCount { get; set; }
        public int DueForReview { get; set; }
    }
}
