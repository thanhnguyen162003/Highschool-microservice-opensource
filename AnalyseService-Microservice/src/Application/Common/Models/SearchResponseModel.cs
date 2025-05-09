using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.NewsModel;
using Application.Common.Models.SearchModel;

namespace Application.Common.Models;

public class SearchResponseModel
{
    public IEnumerable<FlashcardResponseModel> Flashcards { get; set; } = new HashSet<FlashcardResponseModel>();
    public IEnumerable<SubjectResponseModel> Subjects { get; set; } = new HashSet<SubjectResponseModel>();
    public IEnumerable<DocumentResponseModel> Documents { get; set; } = new HashSet<DocumentResponseModel>();
    public IEnumerable<FolderUserResponse> Folders { get; set; } = new HashSet<FolderUserResponse>();
    public IEnumerable<NewsPreviewResponseModel> Tips { get; set; } = new HashSet<NewsPreviewResponseModel>();
}
