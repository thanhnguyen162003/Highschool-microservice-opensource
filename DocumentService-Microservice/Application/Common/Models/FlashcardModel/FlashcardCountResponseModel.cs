namespace Application.Common.Models.FlashcardModel
{
    public class FlashcardCountResponseModel
    {
        public int TotalFlashcard { get; set; }
        public int ThisMonthFlashcard { get; set; }
        public double IncreaseFlashcardPercent { get; set; }
        public int TotalFlashcardDraft { get; set; }
        public int TotalFlashcardOpen { get; set; }
        public int TotalFlashcardLink { get; set; }
    }
}
