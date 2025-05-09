using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EnrollmentProgress : BaseAuditableEntity
    {
        public Guid Id { get; set; }

        public Guid EnrollmentId { get; set; }
        public Guid LessonId { get; set; }

        public virtual Enrollment Enrollment { get; set; } = null!;
        public virtual Lesson Lesson { get; set; } = null!;
    }
}
