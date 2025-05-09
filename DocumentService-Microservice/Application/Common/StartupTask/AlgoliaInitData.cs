using Algolia.Search.Clients;
using Algolia.Search.Models.Search;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.NewsModel;
using Application.Common.Models.SubjectModel;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Application.Common.StartupTask
{
	public class AlgoliaInitData(IServiceScopeFactory serviceScopeFactory) : IHostedService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public async Task StartAsync(CancellationToken cancellationToken)
		{
			var scope = _serviceScopeFactory.CreateScope();
			var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
			var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
			var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

			// Get all data
			var flashcards = mapper.Map<IEnumerable<FlashcardResponseModel>>(mapper.Map<IEnumerable<FlashcardModel>>(await unitOfWork.FlashcardRepository.GetFlashcards()));
			var subjects = mapper.Map<IEnumerable<SubjectResponseModel>>(mapper.Map<IEnumerable<SubjectModel>>(await unitOfWork.SubjectRepository.GetSubjects()));
			var documents = mapper.Map<IEnumerable<DocumentResponseModel>>(await unitOfWork.DocumentRepository.GetDocuments());
            var folders = mapper.Map<IEnumerable<FolderUserResponse>>(await unitOfWork.FolderUserRepository.GetFolders());
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
            var client = new SearchClient(config["AlgoliaSetting:ApplicationId"], config["AlgoliaSetting:WriteApiKey"]);

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
			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			scope.Dispose();
		}

		private async Task<IEnumerable<NewsPreviewResponseModel>?> GetNews()
		{
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(1)
            };
            var response = await client.GetAsync("https://highschool-media-feggakfhgpcacef3.southeastasia-01.azurewebsites.net/api/v1/news?NewsTagId=0193b4bd-9656-78be-4b0a-41e5aadb4c78&PageSize=12&PageNumber=-1");
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

		public Task StopAsync(CancellationToken cancellationToken)
		{
			// Logic for cleanup if necessary
			return Task.CompletedTask;
		}
	}
}
