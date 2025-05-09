using Application.Common.Models.FlashcardContentModel;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardDraftResponseModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? EntityId { get; set; } = null;
    public string? EntityName { get; set; }

    public string FlashcardName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? FlashcardDescription { get; set; }

    public string Status { get; set; } = null!;
    
    public string? CreatedBy { get; set; }
    
    public bool? Created { get; set; }
    
    public List<FlashcardContentResponseModel>? FlashcardContents { get; set; }

    public List<string> Tags { get; set; } = new List<string>();
    
    public int NumberOfFlashcardContent { get; set; }
    public FlashcardType? FlashcardType { get; set; } = Domain.Enums.FlashcardType.Lesson;
}