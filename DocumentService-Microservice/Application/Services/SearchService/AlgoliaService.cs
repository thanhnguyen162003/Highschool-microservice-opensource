using Algolia.Search.Clients;
using Algolia.Search.Models.Search;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.NewsModel;
using Application.Common.Models.SubjectModel;
using Domain.CustomModel;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Application.Services.SearchService
{
    public class AlgoliaService : IAlgoliaService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly SearchClient _clientWrite;
        private readonly SearchClient _clientRead;

        public AlgoliaService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = config;
            _clientWrite = new SearchClient(_config["AlgoliaSetting:ApplicationId"], _config["AlgoliaSetting:WriteApiKey"]);
            _clientRead = new SearchClient(_config["AlgoliaSetting:ApplicationId"], _config["AlgoliaSetting:SearchApiKey"]);
        }

        public async Task<bool> MigrateDataCourse()
        {
            var lessons = _unitOfWork.LessonRepository.GetLessons();
            var chapters = _unitOfWork.ChapterRepository.GetChapters();
            var subjectCurriculums = _unitOfWork.SubjectCurriculumRepository.GetSubjectCurriculums();
            var subjects = _unitOfWork.SubjectRepository.GetSubjectsSearch();
            var data = lessons.Concat(chapters).Concat(subjectCurriculums).Concat(subjects).ToList();

            // Delete index if exist
            await _clientWrite.DeleteIndexAsync("course");

            // Create an index and push data
            try
            {
                // Index setting
                IndexSettings settingCourse = new IndexSettings
                {
                    AttributesForFaceting = new List<string> { "type", "lessonId", "chapterId", "subjectId", "subjectCurriculumId" },
                    SearchableAttributes = new List<string> { "name" },
                    QueryType = QueryType.PrefixLast,
                    MinWordSizefor1Typo = 3,
                    MinWordSizefor2Typos = 7,
                    MinProximity = 5,
                };

                await _clientWrite.SetSettingsAsync("course", settingCourse);

                // Create index
                await _clientWrite.SaveObjectsAsync("course", data);

                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        public async Task<bool> MigrateData()
        {
            // Get all data
            var flashcards = _mapper.Map<IEnumerable<FlashcardResponseModel>>(_mapper.Map<IEnumerable<FlashcardModel>>(await _unitOfWork.FlashcardRepository.GetFlashcards()));
            var subjects = _mapper.Map<IEnumerable<SubjectResponseModel>>(_mapper.Map<IEnumerable<SubjectModel>>(await _unitOfWork.SubjectRepository.GetSubjects()));
            var documents = _mapper.Map<IEnumerable<DocumentResponseModel>>(await _unitOfWork.DocumentRepository.GetDocuments());
            var folders = _mapper.Map<IEnumerable<FolderUserResponse>>(await _unitOfWork.FolderUserRepository.GetFolders());
            var names = flashcards.Select(x => new
            {
                objectID = x.Id,
                name = x.FlashcardName
            }).Concat(subjects.Select(x => new
            {
                objectID = x.Id,
                name = x.SubjectName
            })!)
            .Concat(documents.Select(x => new
            {
                objectID = x.Id,
                name = x.DocumentName
            }))
            .Concat(folders.Select(x => new
            {
                objectID = x.Id,
                name = x.Name
            })!)
            .DistinctBy(x => x.name)
            .ToList();
            var news = await GetNews();

            // Create a client
            var client = new SearchClient(_config["AlgoliaSetting:ApplicationId"], _config["AlgoliaSetting:WriteApiKey"]);

            // Delete index if exist
            await client.DeleteIndexAsync("flashcard");
            await client.DeleteIndexAsync("subject");
            await client.DeleteIndexAsync("document");
            await client.DeleteIndexAsync("name");
            await client.DeleteIndexAsync("folder");
            await client.DeleteIndexAsync("news");

            // Create an index and push data
            try
            {
                //Index setting
                IndexSettings settingFlashcard = new IndexSettings
                {
                    SearchableAttributes = new List<string> { "flashcardName", "flashcardDescription" },
                };

                IndexSettings settingSubject = new IndexSettings
                {
                    SearchableAttributes = new List<string> { "subjectName", "subjectDescription" },
                };

                IndexSettings settingDocument = new IndexSettings
                {
                    SearchableAttributes = new List<string> { "documentName", "documentDescription" },
                };

                IndexSettings settingName = new IndexSettings
                {
                    SearchableAttributes = new List<string> { "name" },
                    QueryType = QueryType.PrefixLast,
                    MinWordSizefor1Typo = 3,
                    MinWordSizefor2Typos = 7,
                    MinProximity = 5
                };

                IndexSettings settingFolder = new IndexSettings
                {
                    SearchableAttributes = new List<string> { "name" },
                };

                IndexSettings settingNews = new IndexSettings
                {
                    SearchableAttributes = new List<string> { "newName" },
                };

                await client.SetSettingsAsync("flashcard", settingFlashcard);
                await client.SetSettingsAsync("subject", settingSubject);
                await client.SetSettingsAsync("document", settingDocument);
                await client.SetSettingsAsync("name", settingName);
                await client.SetSettingsAsync("folder", settingFolder);
                await client.SetSettingsAsync("news", settingNews);

                // Create index
                await client.SaveObjectsAsync("flashcard", flashcards);
                await client.SaveObjectsAsync<SubjectResponseModel>("subject", subjects);
                await client.SaveObjectsAsync<DocumentResponseModel>("document", documents);
                await client.SaveObjectsAsync("name", names);
                await client.SaveObjectsAsync<FolderUserResponse>("folder", folders);
                await client.SaveObjectsAsync<NewsPreviewResponseModel>("news", news);


                Console.WriteLine("Init data sucessfully.");
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<IEnumerable<NewsPreviewResponseModel>?> GetNews()
        {
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(1)
            };
            var response = await client.GetAsync("https://mediaservice.victoriousmeadow-6ebc1027.southeastasia.azurecontainerapps.io/api/v1/news?NewsTagId=01955b98-297c-74da-c44b-9a850f4f2d9e&PageSize=12&PageNumber=-1");
            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<IEnumerable<NewsPreviewResponseModel>>(responseString);

            return result?.Select(obj => new NewsPreviewResponseModel()
            {
                ObjectID = obj.Id.ToString(),
                Id = obj.Id,
                Author = obj.Author,
                NewName = obj.NewName,
                Slug = obj.Slug
            });
        }

        public async Task<bool> AddOrUpdateRecord(IndexName name, string objectID, dynamic model)
        {
            try
            {
                // Create a client
                var response = await _clientWrite.AddOrUpdateObjectAsync(
                          name.ToString(),
                          objectID,
                          model
                        );

                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteRecord(IndexName name, string objectID)
        {
            try
            {
                // Create a client
                var client = new SearchClient(_config["AlgoliaSetting:ApplicationId"], _config["AlgoliaSetting:WriteApiKey"]);

                var response = await client.DeleteObjectAsync(
                          name.ToString(),
                          objectID
                        );

                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> AddOrUpdateCourseRecord(UpdateCourseType type, Guid id, string name)
        {
            try
            {
                // Get all data follow type
                var searchQuery = new SearchQuery(
                    new SearchForHits()
                    {
                        IndexName = "course",
                        Filters = $"{type.ToString()}:{id.ToString()}"
                    }
                );

                var response = await _clientRead.SearchAsync<object>(
                    new SearchMethodParams()
                    {
                        Requests = new List<SearchQuery> { searchQuery }
                    }
                );

                var data = response.Results.ElementAt(0).AsSearchResponse().Hits.Select(x => JsonConvert.DeserializeObject<CourseQueryModel>(x.ToString() ?? "")).ToList();

                // Create/update record
                if (!data.Any())
                {
                    return true;
                }

                switch (type)
                {
                    case UpdateCourseType.lessonId:
                        data.ForEach(x => x.LessonName = name);
                        break;
                    case UpdateCourseType.chapterId:
                        data.ForEach(x => x.ChapterName = name);
                        break;
                    case UpdateCourseType.subjectCurriculumId:
                        data.ForEach(x => x.SubjectCurriculumName = name);
                        break;
                    case UpdateCourseType.subjectId:
                        data.ForEach(x => x.SubjectName = name);
                        break;
                }

                // Push data
                foreach (var item in data)
                {
                    await _clientWrite.AddOrUpdateObjectAsync(
                        "course",
                        item?.ObjectID,
                        item
                        );
                }

                return true;

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteCourseRecord(UpdateCourseType type, Guid id)
        {
            try
            {
                // Get all data follow type
                var searchQuery = new SearchQuery(
                    new SearchForHits()
                    {
                        IndexName = "course",
                        Filters = $"{type.ToString()}:{id.ToString()}"
                    }
                );

                var response = await _clientRead.SearchAsync<object>(
                    new SearchMethodParams()
                    {
                        Requests = new List<SearchQuery> { searchQuery }
                    }
                );

                var data = response.Results.ElementAt(0).AsSearchResponse().Hits.Select(x => JsonConvert.DeserializeObject<CourseQueryModel>(x.ToString() ?? "")).ToList();

                // Create/update record
                if (!data.Any())
                {
                    return true;
                }

                // Push data
                foreach (var item in data)
                {
                    await _clientWrite.DeleteObjectAsync(
                        "course",
                        item?.ObjectID
                        );
                }

                return true;

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
