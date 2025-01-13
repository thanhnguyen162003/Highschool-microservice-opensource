using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class QuestionAnswer : BaseAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string AnswerContent { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public virtual Question Question { get; set; }
    }
}
