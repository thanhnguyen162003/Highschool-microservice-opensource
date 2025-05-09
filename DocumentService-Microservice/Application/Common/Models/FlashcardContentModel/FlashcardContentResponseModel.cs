using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Application.Common.Models.FlashcardContentModel;

public class FlashcardContentResponseModel
{
    public Guid Id { get; set; }
    public Guid FlashcardId { get; set; }
    public string FlashcardContentTerm { get; set; }
    public string? FlashcardContentDefinition { get; set; }
    public string? Image { get; set; }
    public string? FlashcardContentTermRichText { get; set; }
    public string? FlashcardContentDefinitionRichText { get; set; }
    public int Rank { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsStarred { get; set; }
    public bool IsLearned { get; set; }
    public CurrentState CurrentState { get; set; } = CurrentState.NotLearned;
}

public enum CurrentState 
{
    NotLearned,
    Learned,
    DueReviewToday,
}