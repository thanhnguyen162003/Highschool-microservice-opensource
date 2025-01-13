using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserQuizProgress : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? LessonId { get; set; }
        public Guid? ChapterId { get; set; }
        public Guid? SubjectCurriculumId { get; set; }
        public Guid? SubjectId { get; set; }
        /// <summary>
        /// Not use but store anyway
        /// </summary>
        public List<string>? QuestionIds { get; set;}

        public QuestionCategory Category => LessonId != null
            ? QuestionCategory.Lesson
            : (ChapterId != null
                ? QuestionCategory.Chapter
                : (SubjectCurriculumId != null)
                    ? QuestionCategory.SubjectCurriculum
                    : QuestionCategory.Subject);
        public int RecognizingQuestionQuantity { get; set; }
        public float? RecognizingPercent { get; set; }
        public int ComprehensingQuestionQuantity { get; set; }
        public float? ComprehensingPercent { get; set; }
        public int LowLevelApplicationQuestionQuantity { get; set; }
        public float? LowLevelApplicationPercent { get; set; }
        public int HighLevelApplicationQuestionQuantity { get; set; }
        public float? HighLevelApplicationPercent { get; set; }
        public float TotalPercent => (RecognizingPercent ?? 0) + (ComprehensingPercent ?? 0) + (LowLevelApplicationPercent ?? 0) + (HighLevelApplicationPercent ?? 0);
        public float TotalMark => TotalPercent / 100;
        public virtual Subject? Subject { get; set; }
        public virtual SubjectCurriculum? SubjectCurriculum { get; set; }
        public virtual Chapter? Chapter { get; set; }
        public virtual Lesson? Lesson { get; set; }
    }
}
