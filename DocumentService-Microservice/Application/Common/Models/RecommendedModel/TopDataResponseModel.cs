using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.SubjectModel;

namespace Application.Common.Models.RecommendedModel;

public class TopDataResponseModel
{
    public List<SubjectResponseModel> subjects { get; set; }
    public List<DocumentResponseModel> documents { get; set; }
    public List<FlashcardRecommendResponseModel> flashcards { get; set; }
}