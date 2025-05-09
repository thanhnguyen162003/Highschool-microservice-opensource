namespace Application.Common.Models.FlashcardFolderModel
{
    public class ExisItemFolderUserResponse : FolderUserResponse
    {
        public bool? IsFlashcardInclude { get; set; }
        public bool? IsDocumentInclude { get; set; }
    }
}
