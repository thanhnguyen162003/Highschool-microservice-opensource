using Domain.Enums;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardUpdateRequestModel
{
    public string? FlashcardName { get; set; }
    public Guid? EntityId { get; set; }
    public FlashcardType FlashcardType { get; set; } = Domain.Enums.FlashcardType.Lesson;
    public string? FlashcardDescription { get; set; }
    public string? Status { get; set; }
    public List<string>? Tags { get; set; } = new List<string>();
}