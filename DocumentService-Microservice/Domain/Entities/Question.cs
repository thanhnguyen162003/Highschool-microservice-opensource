using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Question : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public Guid? LessonId { get; set; }
        public Guid? ChapterId { get; set; }
        public Guid? SubjectCurriculumId { get; set; }
        public Guid? SubjectId { get; set; }
        public Difficulty Difficulty { get; set; }
        public QuestionType QuestionType { get; set; }
        public QuestionCategory Category => LessonId != null 
            ? QuestionCategory.Lesson 
            : (ChapterId != null 
                ? QuestionCategory.Chapter 
                : (SubjectCurriculumId != null) 
                    ? QuestionCategory.SubjectCurriculum 
                    : QuestionCategory.Subject);
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public virtual Subject? Subject { get; set; }
        public virtual SubjectCurriculum? SubjectCurriculum { get; set; }
        public virtual Chapter? Chapter { get; set; }
        public virtual Lesson? Lesson { get; set; }
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }
    }
}
