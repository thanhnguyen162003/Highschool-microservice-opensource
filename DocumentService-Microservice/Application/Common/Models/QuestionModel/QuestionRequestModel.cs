using Application.Common.Models.QuestionAnswerModel;
using Domain.Enums;

namespace Application.Common.Models.QuestionModel
{
    public class QuestionRequestModel
    {      
        public QuestionCategory Category { get; set; }
        public Guid CategoryId { get; set; }
        public string QuestionContent { get; set; }
        public Difficulty Difficulty { get; set;}
        public QuestionType QuestionType { get; set; }
        public List<QuestionAnswerRequestModel> QuestionAnswers { get; set; } = new List<QuestionAnswerRequestModel>();
    }
}
