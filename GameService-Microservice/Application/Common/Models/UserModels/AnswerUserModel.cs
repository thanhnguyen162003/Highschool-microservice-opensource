namespace Domain.Models.UserModels
{
    public class AnswerUserModel
    {
        public string Answer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int TimeAverage { get; set; }
        public int Score { get; set; }
    }
}
