using Domain.Enums;

namespace Application.Common.Models.QuestionModel
{
    public class GetQuizRequestModel
    {
        public QuestionCategory QuestionCategory { get; set; }
        public Guid CategoryId { get; set; }
    }

}
