using Application.Common.Models.SearchModel;

namespace Application.Common.Models.FlashcardFolderModel
{
    public class FolderUserResponse : SearchResponseModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? CountFlashCard { get; set; }
        public int? CountDocument { get; set; }
        public string? Visibility { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuthorFolder? Author { get; set; } = new AuthorFolder();
    }
}
