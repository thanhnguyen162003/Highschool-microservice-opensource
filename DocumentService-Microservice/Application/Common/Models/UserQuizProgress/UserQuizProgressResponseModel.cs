using Application.Common.Models.QuestionModel;
using Domain.Enums;

namespace Application.Common.Models.UserQuizProgress
{
    public class UserQuizProgressResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public QuestionCategory Category { get; set; }
        public Guid? LessonId { get; set; }
        public Guid? ChapterId { get; set; }
        public Guid? SubjectCurriculumId { get; set; }
        public Guid? SubjectId { get; set; }
        public int RecognizingQuestionQuantity { get; set; }
        public float? RecognizingPercent { get; set; }
        public int ComprehensingQuestionQuantity { get; set; }
        public float? ComprehensingPercent { get; set; }
        public int LowLevelApplicationQuestionQuantity { get; set; }
        public float? LowLevelApplicationPercent { get; set; }
        public int HighLevelApplicationQuestionQuantity { get; set; }
        public float? HighLevelApplicationPercent { get; set; }
        public float TotalPercent { get; set; }
        public float TotalMark { get; set; }
        public List<QuestionResponseModel> Questions { get; set; } = new List<QuestionResponseModel>();
    }

}
