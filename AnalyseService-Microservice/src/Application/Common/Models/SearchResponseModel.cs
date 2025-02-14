using Application.Common.Models.SearchModel;

namespace Application.Common.Models;

public class SearchResponseModel
{
    public IEnumerable<FlashcardResponseModel> Flashcards { get; set; } = new HashSet<FlashcardResponseModel>();
    public IEnumerable<SubjectResponseModel> Subjects { get; set; } = new HashSet<SubjectResponseModel>();
    public IEnumerable<DocumentResponseModel> Documents { get; set; } = new HashSet<DocumentResponseModel>();
}
