namespace Application.Common.Models.DaprModel.Flashcard
{
    public class FlashcardTipsResponseDapr
    {
        public List<string> FlaschcardId { get; set; } = new List<string>();
        public List<string> FlaschcardName { get; set; } = new List<string>();
        public List<string> FlaschcardSlug { get; set; } = new List<string>();
    }
}
