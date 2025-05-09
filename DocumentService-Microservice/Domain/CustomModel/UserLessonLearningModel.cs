using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CustomModel
{
    public class UserLessonLearningModel
    {
        public Guid UserId { get; set; }
        public List<DateTime> LessonLearnDate { get; set; } = new List<DateTime>();
    }
}
