namespace Application.Common.Models.FlashcardContentModel;

public class FlashcardContentUpdateRequestModel
{
    public string? FlashcardContentTerm { get; set; }
    public string? FlashcardContentDefinition { get; set; }
    public string? Image { get; set; }
    public string? FlashcardContentTermRichText { get; set; }
    public string? FlashcardContentDefinitionRichText { get; set; }
    public Guid? Id { get; set; }
}