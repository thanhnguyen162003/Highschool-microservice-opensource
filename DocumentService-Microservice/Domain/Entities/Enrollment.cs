using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Enrollment : BaseAuditableEntity
    {
        public Guid Id { get; set; }

        public Guid BaseUserId { get; set; }

        public Guid SubjectCurriculumId { get; set; }

        public virtual ICollection<EnrollmentProgress> EnrollmentProgresses { get; set; } = new List<EnrollmentProgress>();

        public virtual SubjectCurriculum SubjectCurriculum { get; set; } = null!;
    }
}
