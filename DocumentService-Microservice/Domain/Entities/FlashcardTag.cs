using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FlashcardTag : BaseAuditableEntity
    {
        public Guid FlashcardId { get; set; }
        public Guid TagId { get; set; }

        public virtual Flashcard Flashcard { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
