namespace Application.Common.Models.FlashcardContentModel;

public class FlashcardContentCreateRequestModel
{
    public string? FlashcardContentTerm { get; set; }
    public string? FlashcardContentDefinition { get; set; }
    public string? Image { get; set; }
    public string? FlashcardContentTermRichText { get; set; }
    public string? FlashcardContentDefinitionRichText { get; set; }
    public int? Rank { get; set; }
}