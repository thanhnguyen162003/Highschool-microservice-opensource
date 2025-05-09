using Algolia.Search.Clients;
using Algolia.Search.Models.Search;
using Application.Common.Models;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.NewsModel;
using Application.Common.Models.SearchModel;
using Application.Constants;
using Domain.CustomModel;
using Domain.Enums;
using Domain.QueriesFilter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Services.Search;

public class SearchService : ISearchService
{
    private readonly SearchClient _client;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public SearchService(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _client = new SearchClient(_configuration["AlgoliaSetting:ApplicationId"], _configuration["AlgoliaSetting:SearchApiKey"]);
        _mapper = mapper;
    }

    public async Task<SearchResponseModel> SearchAll(string value, int page, int eachPage)
    {

        return new SearchResponseModel
        {
            Flashcards = await SearchFlashCard(value, page, eachPage),
            Subjects = await SearchSubject(value, page, eachPage),
            Documents = await SearchDocument(value, page, eachPage),
            Folders = await SearchFolder(value, page, eachPage),
            Tips = await SearchTips(value, page, eachPage)
        };
    }

    public async Task<IEnumerable<CourseSearchResponseModel>> SearchCourseName(SearchCourseType type, string value, int limit)
    {
        // Use multiple query to search in multiple index
        // Query by sort type

        var searchQueries = new List<SearchQuery>();

        for(int i = 1; i <= (int)type; i++)
        {
            searchQueries.Add(new SearchQuery(new SearchForFacets
            {
                IndexName = IndexSearchConstant.Course,
                Filters = $"type:{Enum.Parse<SearchCourseType>(i.ToString()).ToString()}",
                Query = value,
                Page = 0,
                HitsPerPage = limit,
            }));
        }

        // Search
        var result = await _client.SearchAsync<object>(
            new SearchMethodParams
            {
                Requests = searchQueries.ToList()
            }
        );

        var objects = new List<object>();
        for (int i = 0; i < (int)type; i++)
        {
            objects.AddRange(result.Results.ElementAt(i).AsSearchResponse().Hits);
        };

        return objects.Select(hit =>
        {
            var doc = JsonConvert.DeserializeObject<CourseQueryModel>(hit.ToString());

            return _mapper.Map<CourseSearchResponseModel>(doc);
        });
    } 

    public async Task<IEnumerable<SubjectResponseModel>> SearchSubject(string value, int page, int eachPage)
    {
        // Create a search query
        var searchQuery = new SearchQuery
        (
            new SearchForHits
            {
                IndexName = IndexSearchConstant.Subject,
                Query = value,
                Page = page,
                HitsPerPage = eachPage
            }
        );

        // Search
        var result = await _client.SearchAsync<object>(
            new SearchMethodParams
            {
                Requests = new List<SearchQuery>
                {
                        searchQuery
                }
            }
        );

        var subjects = result.Results.ElementAt(0).AsSearchResponse().Hits;

        return subjects.Select(hit =>
            {
                var subject = JsonConvert.DeserializeObject<SubjectResponseModel>(hit.ToString());

                var hitObj = JsonConvert.DeserializeObject<JObject>(hit.ToString());
                if (hitObj["_highlightResult"] != null)
                {
                    subject.HighlightResult = hitObj["_highlightResult"].ToObject<Application.Common.Models.SearchModel.SubjectHighlightResult>();
                }

                return new SubjectResponseModel
                {
                    Id = subject.Id,
                    Like = subject.Like,
                    Slug = subject.Slug,
                    CreatedAt = subject.CreatedAt,
                    UpdatedAt = subject.UpdatedAt,
                    Image = subject.Image,
                    Information = subject.Information,
                    View = subject.View,
                    CategoryName = subject.CategoryName,
                    NumberEnrollment = subject.NumberEnrollment,
                    SubjectCode = subject.SubjectCode,
                    SubjectDescription = subject.SubjectDescription,
                    SubjectName = subject.SubjectName,
                    NumberOfChapters = subject.NumberOfChapters,
                    HighlightResult = subject.HighlightResult
                };
            });
    }

    public async Task<IEnumerable<DocumentResponseModel>> SearchDocument(string value, int page, int eachPage)
    {
        // Create a search query
        var searchQuery = new SearchQuery
        (
            new SearchForHits
            {
                IndexName = IndexSearchConstant.Document,
                Query = value,
                Page = page,
                HitsPerPage = eachPage
            }
        );

        // Search
        var result = await _client.SearchAsync<object>(
            new SearchMethodParams
            {
                Requests = new List<SearchQuery>
                {
                        searchQuery
                }
            }
        );

        var documents = result.Results.ElementAt(0).AsSearchResponse().Hits;

        return documents.Select(hit =>
        {
            var doc = JsonConvert.DeserializeObject<DocumentResponseModel>(hit.ToString());

            var hitObj = JsonConvert.DeserializeObject<JObject>(hit.ToString());
            if (hitObj["_highlightResult"] != null)
            {
                doc.HighlightResult = hitObj["_highlightResult"].ToObject<Application.Common.Models.SearchModel.DocumentHighlightResult>();
            }

            return new DocumentResponseModel
            {
                Id = doc.Id,
                Like = doc.Like,
                CreatedAt = doc.CreatedAt,
                UpdatedAt = doc.UpdatedAt,
                UpdatedBy = doc.UpdatedBy,
                CreatedBy = doc.CreatedBy,
                View = doc.View,
                Subject = doc.Subject,
                Download = doc.Download,
                Category = doc.Category,
                DocumentDescription = doc.DocumentDescription,
                DocumentName = doc.DocumentName,
                DocumentSlug = doc.DocumentSlug,
                DocumentYear = doc.DocumentYear,
                HighlightResult = doc.HighlightResult
            };
        });
    }

    public async Task<IEnumerable<FlashcardResponseModel>> SearchFlashCard(string value, int page, int eachPage)
    {
        // Create a search query
        var searchQuery = new SearchQuery
        (
            new SearchForHits
            {
                IndexName = IndexSearchConstant.Flashcard,
                Query = value,
                Page = page,
                HitsPerPage = eachPage
            }
        );

        // Search
        var result = await _client.SearchAsync<object>(
            new SearchMethodParams
            {
                Requests = new List<SearchQuery>
                {
                        searchQuery
                }
            }
        );

        var flashcards = result.Results.ElementAt(0).AsSearchResponse().Hits;

        return flashcards.Select(hit =>
        {
            var card = JsonConvert.DeserializeObject<FlashcardResponseModel>(hit.ToString());

            var hitObj = JsonConvert.DeserializeObject<JObject>(hit.ToString());
            if (hitObj["_highlightResult"] != null)
            {
                card.HighlightResult = hitObj["_highlightResult"].ToObject<Application.Common.Models.SearchModel.FlashcardHighlightResult>();
            }

            return new FlashcardResponseModel
            {
                Id = card.Id,
                Like = card.Like,
                Slug = card.Slug,
                Star = card.Star,
                Status = card.Status,
                CreatedAt = card.CreatedAt,
                CreatedBy = card.CreatedBy,
                FlashcardDescription = card.FlashcardDescription,
                FlashcardName = card.FlashcardName,
                SubjectId = card.SubjectId,
                UpdatedAt = card.UpdatedAt,
                UpdatedBy = card.UpdatedBy,
                UserId = card.UserId,
                NumberOfFlashcardContent = flashcards.Count(),
                HighlightResult = card.HighlightResult
            };
        });
    } 

    public async Task<IEnumerable<string>> SearchName(string value)
    {
        // Create a search query
        var searchQuery = new SearchQuery
        (
            new SearchForHits
            {
                IndexName = IndexSearchConstant.Name,
                Query = value
            }
        );

        // Search
        var result = await _client.SearchAsync<object>(
            new SearchMethodParams
            {
                Requests = new List<SearchQuery>
                {
                        searchQuery
                }
            }
        );

        var names = result.Results.ElementAt(0).AsSearchResponse().Hits;

        return names.Select(hit =>
        {
            var doc = JsonConvert.DeserializeObject<NameResponseModel>(hit.ToString());

            return doc.Name;
        });
    }

    public async Task<IEnumerable<FolderUserResponse>> SearchFolder(string value, int page, int eachPage)
    {
        // Create a search query
        var searchQuery = new SearchQuery
        (
            new SearchForHits
            {
                IndexName = IndexSearchConstant.Folder,
                Query = value,
                Page = page,
                HitsPerPage = eachPage
            }
        );

        // Search
        var result = await _client.SearchAsync<object>(
            new SearchMethodParams
            {
                Requests = new List<SearchQuery>
                {
                        searchQuery
                }
            }
        );

        var folders = result.Results.ElementAt(0).AsSearchResponse().Hits;

        return folders.Select(hit =>
        {
            var doc = JsonConvert.DeserializeObject<FolderUserResponse>(hit.ToString());

            return doc!;
        });
    }

    public async Task<IEnumerable<NewsPreviewResponseModel>> SearchTips(string value, int page, int eachPage)
    {
        // Create a search query
        var searchQuery = new SearchQuery
        (
            new SearchForHits
            {
                IndexName = IndexSearchConstant.News,
                Query = value,
                Page = page,
                HitsPerPage = eachPage
            }
        );

        // Search
        var result = await _client.SearchAsync<object>(
            new SearchMethodParams
            {
                Requests = new List<SearchQuery>
                {
                        searchQuery
                }
            }
        );

        var tips = result.Results.ElementAt(0).AsSearchResponse().Hits;

        return tips.Select(hit =>
        {
            var doc = JsonConvert.DeserializeObject<NewsPreviewResponseModel>(hit.ToString());

            return doc!;
        });
    }
}
