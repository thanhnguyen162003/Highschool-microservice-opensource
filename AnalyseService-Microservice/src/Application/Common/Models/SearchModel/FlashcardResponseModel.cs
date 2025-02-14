namespace Application.Common.Models.SearchModel;

public class FlashcardResponseModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid SubjectId { get; set; }

    public string FlashcardName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? FlashcardDescription { get; set; }

    public string Status { get; set; } = null!;

    public int? Like { get; set; }

    public double? Star { get; set; }

    public string? CreatedBy { get; set; }

    public string? CreatedAt { get; set; }

    public string? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public int NumberOfFlashcardContent { get; set; }

    public FlashcardHighlightResult? HighlightResult { get; set; }
}

public class FlashcardHighlightResult
{
    public FlashcardHighlightField FlashcardName { get; set; }
    public FlashcardHighlightField FlashcardDescription { get; set; }
}

public class FlashcardHighlightField
{
    public string Value { get; set; }
    public string MatchLevel { get; set; }
    public bool FullyHighlighted { get; set; }
    public List<string> MatchedWords { get; set; }
}
