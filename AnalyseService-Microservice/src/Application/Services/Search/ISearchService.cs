using Application.Common.Models;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.NewsModel;
using Application.Common.Models.SearchModel;
using Domain.CustomModel;
using Domain.Enums;
using Domain.QueriesFilter;

namespace Application.Services.Search;

public interface ISearchService
{
    Task<IEnumerable<FlashcardResponseModel>> SearchFlashCard(string value, int page, int eachPage);
    Task<IEnumerable<SubjectResponseModel>> SearchSubject(string value, int page, int eachPage);
    Task<IEnumerable<DocumentResponseModel>> SearchDocument(string value, int page, int eachPage);
    Task<SearchResponseModel> SearchAll(string value, int page, int eachPage);
    Task<IEnumerable<string>> SearchName(string value);
    Task<IEnumerable<NewsPreviewResponseModel>> SearchTips(string value, int page, int eachPage);
    Task<IEnumerable<FolderUserResponse>> SearchFolder(string value, int page, int eachPage);
    Task<IEnumerable<CourseSearchResponseModel>> SearchCourseName(SearchCourseType type, string value, int limit);
}
