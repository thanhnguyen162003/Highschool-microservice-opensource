using Algolia.Search.Clients;
using Algolia.Search.Models.Search;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.SubjectModel;
using Domain.CustomModel;
using Infrastructure.Repositories.Interfaces;

namespace Application.Common.StartupTask
{
	public class AlgoliaInitData : IHostedService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public AlgoliaInitData(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

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

			// Create a client
			var client = new SearchClient(config["AlgoliaSetting:ApplicationId"], config["AlgoliaSetting:WriteApiKey"]);

			// Delete index if exist
			await client.DeleteIndexAsync("flashcard");
			await client.DeleteIndexAsync("subject");
			await client.DeleteIndexAsync("document");

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

				await client.SetSettingsAsync("flashcard", settingFlashcard);
				await client.SetSettingsAsync("subject", settingSubject);
				await client.SetSettingsAsync("document", settingDocument);

				// Create index
				await client.SaveObjectsAsync<FlashcardResponseModel>("flashcard", flashcards);
				await client.SaveObjectsAsync<SubjectResponseModel>("subject", subjects);
				await client.SaveObjectsAsync<DocumentResponseModel>("document", documents);

				Console.WriteLine("Init data sucessfully.");
			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			scope.Dispose();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			// Logic for cleanup if necessary
			return Task.CompletedTask;
		}
	}
}
