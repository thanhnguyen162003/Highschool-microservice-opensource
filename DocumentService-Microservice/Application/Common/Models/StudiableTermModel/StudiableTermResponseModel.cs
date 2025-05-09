using Application.Common.Models.FlashcardContentModel;

namespace Application.Common.Models.StudiableTermModel;

public class StudiableTermResponseModel
{
    public FlashcardContentResponseModel FlashcardContent { get; set; }
    public string? Mode { get; set; }
}