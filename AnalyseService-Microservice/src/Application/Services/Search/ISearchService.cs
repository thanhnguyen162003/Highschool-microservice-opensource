using Application.Common.Models;
using Application.Common.Models.SearchModel;

namespace Application.Services.Search;

public interface ISearchService
{
    Task<IEnumerable<FlashcardResponseModel>> SearchFlashCard(string value);
    Task<IEnumerable<SubjectResponseModel>> SearchSubject(string value);
    Task<IEnumerable<DocumentResponseModel>> SearchDocument(string value);
    Task<SearchResponseModel> SearchAll(string value);
}
