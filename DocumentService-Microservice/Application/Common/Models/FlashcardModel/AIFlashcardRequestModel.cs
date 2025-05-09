using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.FlashcardModel
{
	public class AIFlashcardRequestModel
	{

		public IFormFile? FileRaw { get; set; }

		public string? TextRaw { get; set; }

		public string? Note { get; set; }

		public int? NumberFlashcardContent { get; set; }

		public string? LevelHard { get; set; }

		public string? FrontTextLong { get; set; }

		public string? BackTextLong { get; set; }

	}
}
