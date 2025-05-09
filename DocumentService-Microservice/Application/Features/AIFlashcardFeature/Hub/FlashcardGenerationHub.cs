using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models.FlashcardModel;
using Microsoft.AspNetCore.SignalR;

namespace Application.Features.AIFlashcardFeature.Hub
{
	public class FlashcardGenerationHub : Microsoft.AspNetCore.SignalR.Hub
	{
		public async Task UpdateFlashcardGenerationProgress(string userId, string message, int progress, int total)
		{
			await Clients.User(userId).SendAsync("ReceiveFlashcardGenerationProgress", message, progress, total);
		}

		public async Task SendGeneratedFlashcard(string userId, FlashcardContentResponseModel flashcard)
		{
			await Clients.User(userId).SendAsync("ReceiveGeneratedFlashcard", flashcard);
		}

		public async Task CompleteFlashcardGeneration(string userId, FlashcardDraftResponseModel flashcardDraft)
		{
			await Clients.User(userId).SendAsync("FlashcardGenerationCompleted", flashcardDraft);
		}

		public async Task NotifyGenerationError(string userId, string errorMessage)
		{
			await Clients.User(userId).SendAsync("FlashcardGenerationError", errorMessage);
		}
	}
}
