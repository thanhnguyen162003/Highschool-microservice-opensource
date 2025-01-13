using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FolderUser
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Visibility { get; set; }

        public IEnumerable<FlashcardFolder> FlashcardFolders { get; set; } = new List<FlashcardFolder>();
        public IEnumerable<DocumentFolder> DocumentFolders { get; set; } = new List<DocumentFolder>();
    }
}
