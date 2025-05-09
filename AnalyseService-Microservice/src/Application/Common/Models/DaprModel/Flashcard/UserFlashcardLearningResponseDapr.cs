namespace Application.Common.Models.DaprModel.Flashcard
{
    public class UserFlashcardLearningResponseDapr
    {
        public List<UserFlashcardLearningDapr> UserFlashcardLearning { get; set; } = new List<UserFlashcardLearningDapr>();
    }
    public class UserFlashcardLearningDapr
    {
        public string UserId { get; set; }
        public string FlashcardId { get; set; }
        public string FlashcardContentId { get; set; }
        public List<string> LastReviewDateHistory { get; set; } = new List<string>();
        public List<double> TimeSpentHistory { get; set; } = new List<double>();
    }
}
