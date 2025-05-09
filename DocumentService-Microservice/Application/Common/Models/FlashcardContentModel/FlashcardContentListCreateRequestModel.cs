using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.FlashcardContentModel
{
	public class FlashcardContentListCreateRequestModel
	{
		[Required]
		public string? FlashcardContentTerm { get; set; }
		[Required]
		public string? FlashcardContentDefinition { get; set; }
		public string? Image { get; set; }
		public string? FlashcardContentTermRichText { get; set; }
		public string? FlashcardContentDefinitionRichText { get; set; }
	}
}
