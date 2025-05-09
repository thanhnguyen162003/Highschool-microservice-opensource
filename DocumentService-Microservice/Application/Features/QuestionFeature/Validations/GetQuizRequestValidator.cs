using Application.Common.Models.QuestionModel;

namespace Application.Features.QuestionFeature.Validations
{
    public class GetQuizRequestValidator : AbstractValidator<GetQuizRequestModel>
    {
        public GetQuizRequestValidator()
        {
            RuleFor(x => x.QuestionCategory)
                .NotNull().WithMessage("Loại câu hỏi không được để trống.")
                .IsInEnum().WithMessage("Loại câu hỏi không hợp lệ.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Danh mục không được để trống.");
        }
    }

}
