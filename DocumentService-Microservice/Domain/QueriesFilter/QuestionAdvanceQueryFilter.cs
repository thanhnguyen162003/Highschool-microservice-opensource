using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.QueriesFilter
{
    public class QuestionAdvanceQueryFilter
    {
        public Guid? LessonId { get; set; }
        public Guid? ChapterId { get; set; }
        public Guid? SubjectCurriculumId { get; set; }
        public Guid? SubjectId { get; set; }
        public Difficulty? Difficulty { get; set; }
        public QuestionType? QuestionType { get; set; }
        public QuestionCategory? Category { get; set; }
        public string? Search { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
