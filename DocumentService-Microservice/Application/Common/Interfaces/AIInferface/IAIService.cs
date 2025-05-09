using Application.Common.Models;
using Application.Common.Models.FlashcardContentModel;

namespace Application.Common.Interfaces.AIInferface
{
	public interface IAIService
	{
		Task<ResponseModel> GenerateFlashcardContent(
			string? note,
			IFormFile? fileRaw,
			string? textRaw,
			int? numberFlashcard,
			string? levelHard,
			string? frontTextLong,
			string? backTextLong);

        Task<FlashcardContentAIAssessModel> AssessAnswer(string term, string definition, string userAnswer);

		IAsyncEnumerable<FlashcardContentCreateRequestModel> GenerateFlashcardContentStreamAsync(string? note, IFormFile? fileRaw, string? textRaw,
			int? numberFlashcard, string? levelHard, string? frontTextLong, string? backTextLong, CancellationToken cancellationToken);
	}
}
