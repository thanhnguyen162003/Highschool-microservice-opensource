using Domain.Enums;

namespace Application.Common.Models.UserQuizProgress
{
    public class ClearUserQuizProgressRequestModel
    {
        public QuestionCategory Category { get; set; }
        public Guid CategoryId { get; set; }
    }
}
