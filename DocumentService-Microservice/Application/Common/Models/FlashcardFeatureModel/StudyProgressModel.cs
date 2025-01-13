namespace Application.Common.Models.FlashcardFeatureModel;

public class StudyProgressModel
{
    public int MasteredTerms { get; set; }
    public int TotalTerms { get; set; }
    public int UnLearnTerm { get; set; }
    public int StudyingTermNumber { get; set; }
    public double MasteryPercentage { get; set; }
    public List<MasteredTerm> MasteredTermsDetail { get; set; } = new List<MasteredTerm>();
    public List<StudyingTerm> StudyingTerms { get; set; } = new List<StudyingTerm>();
    public class MasteredTerm
    {
        public Guid Id { get; set; } 
        public string Term { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
    }
    public class StudyingTerm
    {
        public Guid Id { get; set; } 
        public string Term { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        // public int NumberOfCorrect { get; set; }
    }
}

