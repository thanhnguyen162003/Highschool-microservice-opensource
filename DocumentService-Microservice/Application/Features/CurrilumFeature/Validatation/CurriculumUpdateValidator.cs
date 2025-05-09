using Application.Common.Models.CurriculumModel;

namespace Application.Features.CurrilumFeature.Validatation
{
	public class CurriculumUpdateValidator : AbstractValidator<CurriculumUpdateRequestModel>
	{
		public CurriculumUpdateValidator()
		{
			RuleFor(v => v.CurriculumName)
				.MaximumLength(200).WithMessage("Tên của chương trình học không được vượt quá 200 ký tự");

		}
	}
}
