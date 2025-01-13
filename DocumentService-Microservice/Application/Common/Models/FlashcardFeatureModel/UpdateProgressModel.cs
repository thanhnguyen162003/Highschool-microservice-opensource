namespace Application.Common.Models.FlashcardFeatureModel;

public class UpdateProgressModel
{
    public Guid FlashcardId { get; init; }
    public Guid FlashcardContentId { get; init; }
    public bool IsCorrect { get; init; }
}