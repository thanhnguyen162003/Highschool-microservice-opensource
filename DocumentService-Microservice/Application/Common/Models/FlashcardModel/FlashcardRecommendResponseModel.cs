using Domain.Enums;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardRecommendResponseModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? EntitySlug { get; set; }
    
    public FlashcardType? FlashcardType { get; set; } = Domain.Enums.FlashcardType.Lesson;

    public string? Grade { get; set; }

    public string FlashcardName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? FlashcardDescription { get; set; }

    public bool? Created { get; set; }

    public string Status { get; set; } = null!;

    public double? Star { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public string? CreatedAt { get; set; }

    public string? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
    
    public int NumberOfFlashcardContent { get; set; }

    public List<string> Tags { get; set; } = new List<string>();
}