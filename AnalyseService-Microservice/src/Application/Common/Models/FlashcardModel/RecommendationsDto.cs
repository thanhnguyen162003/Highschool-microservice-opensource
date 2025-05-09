namespace Application.Common.Models.FlashcardModel;

public class RecommendationsDto
{
    public List<string> StudyRecommendations { get; set; } = new();
    public List<Guid> PriorityReviewCards { get; set; } = new();
    public string OptimalStudySchedule { get; set; }
    public List<string> ImprovementAreas { get; set; } = new();
}
