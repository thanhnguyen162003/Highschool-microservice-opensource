namespace Application.Common.Models.FlashcardFeatureModel
{
	public class FlashcardContentQuestionModel
	{
		public string Term { get; set; } = string.Empty;
		public string CorrectAnswer { get; set; } = string.Empty;
		public Guid CorrectAnswerId { get; set; } 
		public List<AnswerOption> Options { get; set; } = new List<AnswerOption>();
	}
	public class AnswerOption
	{
		public Guid Id { get; set; } 
		public string Definition { get; set; } = string.Empty; 
	}
}
