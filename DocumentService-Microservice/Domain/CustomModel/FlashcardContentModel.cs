namespace Domain.CustomModel;

public class FlashcardContentModel
{
    public Guid? Id { get; set; }

    public Guid? FlashcardId { get; set; }

    public string? FlashcardContentTerm { get; set; }

    public string? FlashcardContentDefinition { get; set; }

    public string? Image { get; set; }
    
    public string? FlashcardContentTermRichText { get; set; }
    
    public string? FlashcardContentDefinitionRichText { get; set; }
    
    public int? Rank { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    
    public string? UpdatedBy { get; set; }
}