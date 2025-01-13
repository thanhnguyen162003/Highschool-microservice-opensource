using Application.Common.Models;
using Application.Common.Models.SearchModel;
using Application.Services.Search;

namespace Application.Features.SearchFeature.Queries;

public record SearchQuery : IRequest<object>
{
    public string? type { get; init; }
    public string Value { get; init; } = "";
}

public class SearchQueryHandler : IRequestHandler<SearchQuery, object>
{
    private readonly ISearchService _searchService;

    public SearchQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<object> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.type))
        {
            // Search all returns a common response
            return await _searchService.SearchAll(request.Value);
        }
        else if (request.type == "flashcard")
        {
            // Return specific type for flashcard
            IEnumerable<FlashcardResponseModel> flashcardResults = await _searchService.SearchFlashCard(request.Value);
            return flashcardResults;
        }
        else if (request.type == "subject")
        {
            // Return specific type for subject
            IEnumerable<SubjectResponseModel> subjectResults = await _searchService.SearchSubject(request.Value);
            return subjectResults;
        }
        else if (request.type == "document")
        {
            // Return specific type for document
            IEnumerable<DocumentResponseModel> documentResults = await _searchService.SearchDocument(request.Value);
            return documentResults;
        }
        else
        {
            throw new Exception("Type not found");
        }
    }
}
