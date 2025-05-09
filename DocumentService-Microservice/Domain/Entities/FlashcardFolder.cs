using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FlashcardFolder
    {
        public Guid Id { get; set; }
        public Guid FlashcardId { get; set; }
        public Guid FolderId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Flashcard? Flashcard { get; set; }
        public virtual FolderUser? Folder { get; set; }
    }
}
