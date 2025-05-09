using Application.Common.Models;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.NewsModel;
using Application.Common.Models.SearchModel;
using Application.Services.Search;
using Domain.CustomModel;
using Domain.Enums;

namespace Application.Features.SearchFeature.Queries;

public record SearchQuery : IRequest<object>
{
    public SearchType Type { get; set; }
    public string? Value { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}

public class SearchQueryHandler(ISearchService searchService) : IRequestHandler<SearchQuery, object>
{
    private readonly ISearchService _searchService = searchService;

    public async Task<object> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        if (request.PageNumber <= 0)
        {
            request.PageNumber = 0;
        } else
        {
            request.PageNumber -= 1;
        }

        request.Value = request.Value ?? string.Empty;
        if (request.Type == SearchType.All)
        {
            // Search all returns a common response
            return await _searchService.SearchAll(request.Value, request.PageNumber, request.PageSize);
        }
        else if (request.Type == SearchType.Flashcard)
        {
            // Return specific type for flashcard
            IEnumerable<FlashcardResponseModel> flashcardResults = await _searchService.SearchFlashCard(request.Value, request.PageNumber, request.PageSize);

            return PagedList<Object>.Create(flashcardResults, request.PageNumber, request.PageSize);
        }
        else if (request.Type == SearchType.Subject)
        {
            // Return specific type for subject
            IEnumerable<SubjectResponseModel> subjectResults = await _searchService.SearchSubject(request.Value, request.PageNumber, request.PageSize);

            return PagedList<Object>.Create(subjectResults, request.PageNumber, request.PageSize);
        }
        else if (request.Type == SearchType.Document)
        {
            // Return specific type for document
            IEnumerable<DocumentResponseModel> documentResults = await _searchService.SearchDocument(request.Value, request.PageNumber, request.PageSize);

            return PagedList<Object>.Create(documentResults, request.PageNumber, request.PageSize);
        } else if (request.Type == SearchType.Name)
        {
            // Return specific type for name
            IEnumerable<string> nameResults = await _searchService.SearchName(request.Value);

            return PagedList<Object>.Create(nameResults, request.PageNumber, request.PageSize);
        } else if (request.Type == SearchType.Folder)
        {
            IEnumerable<FolderUserResponse> folderResults = await _searchService.SearchFolder(request.Value, request.PageNumber, request.PageSize);

            return PagedList<Object>.Create(folderResults, request.PageNumber, request.PageSize);
        } else if (request.Type == SearchType.News)
        {
            IEnumerable<NewsPreviewResponseModel> newsResults = await _searchService.SearchTips(request.Value, request.PageNumber, request.PageSize);
            return PagedList<Object>.Create(newsResults, request.PageNumber, request.PageSize);
        }

        return new List<object>();
    }
}
