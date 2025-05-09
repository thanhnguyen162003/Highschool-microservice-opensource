using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CustomModel
{
    public class EnrollmentProgressModel
    {
        public float SubjectProgressPercent { get; set; }
        public EntitySimpleModel? LastViewedChapter { get; set; }
        public EntitySimpleModel? LastViewedLesson { get; set; }
    }
}
