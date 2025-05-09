using Application.Common.Models.QuestionAnswerModel;
using Domain.Enums;

namespace Application.Common.Models.QuestionModel
{
    public class QuestionResponseModel
    {
        public Guid Id { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public Guid? LessonId { get; set; }
        public Guid? ChapterId { get; set; }
        public Guid? SubjectCurriculumId { get; set; }
        public Guid? SubjectId { get; set; }
        public Difficulty Difficulty { get; set; }
        public QuestionType QuestionType { get; set; }
        public QuestionCategory Category { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Thêm danh sách câu trả lời
        public List<QuestionAnswerResponseModel> Answers { get; set; } = new();
    }

}
