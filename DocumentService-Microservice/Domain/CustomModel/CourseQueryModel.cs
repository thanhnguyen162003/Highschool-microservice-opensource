using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CustomModel
{
    public class CourseQueryModel
    {
        public string ObjectID { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid? LessonId { get; set; }
        public string? LessonName { get; set; }
        public Guid? ChapterId { get; set; }
        public string? ChapterName { get; set; }
        public Guid? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public Guid? SubjectCurriculumId { get; set; }
        public string? SubjectCurriculumName { get; set; }
    }
}
