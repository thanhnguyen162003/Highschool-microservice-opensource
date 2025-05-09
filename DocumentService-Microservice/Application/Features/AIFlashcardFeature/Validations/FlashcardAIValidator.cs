using Application.Common.Models.FlashcardModel;

namespace Application.Features.AIFlashcardFeature.Validations
{
	public class FlashcardAIValidator : AbstractValidator<AIFlashcardRequestModel>
	{
		public FlashcardAIValidator()
		{
			RuleFor(v => v.FrontTextLong)
				.NotEmpty().WithMessage("Front text long is required.")
				.Must(BeAValidTextLong).WithMessage("TextLong must be one of: very short, short, medium, long, very long");

			RuleFor(v => v.BackTextLong)
				.NotEmpty().WithMessage("Back text long is required.")
				.Must(BeAValidTextLong).WithMessage("TextLong must be one of: very short, short, medium, long, very long");

			RuleFor(v => v.LevelHard)
				.Must(BeAValidLevel).WithMessage("Level must be one of: easy, normal, hard");
		}
		private bool BeAValidTextLong(string? textLong)
		{
			var allowedText = new[] { "very short", "short", "long", "medium", "very long" };
			return allowedText.Contains(textLong);
		}
		private bool BeAValidLevel(string? level)
		{
			var allowedLevel = new[] { "easy", "normal", "hard" };
			return allowedLevel.Contains(level);
		}
	}
}
