namespace Domain.Entities
{
    public class DocumentFolder
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Guid FolderId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Document? Document { get; set; }
        public virtual FolderUser? Folder { get; set; }
    }
}
