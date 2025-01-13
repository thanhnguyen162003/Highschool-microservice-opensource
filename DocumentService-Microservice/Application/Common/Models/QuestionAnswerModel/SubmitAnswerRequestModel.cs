using Domain.Enums;

namespace Application.Common.Models.QuestionAnswerModel
{
    public class SubmitAnswerRequestModel
    {
        public QuestionCategory QuestionCategory { get; set; }
        public Guid CategoryId { get; set; }
        public List<QuizAnswerRequestModel> Answers { get; set; } = new List<QuizAnswerRequestModel>();
    }
}
