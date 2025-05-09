using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models.UserQuizProgress;

namespace Application.Features.UserQuizProgressFeature.Validators
{
    public class ClearUserQuizProgressValidator : AbstractValidator<ClearUserQuizProgressRequestModel>
    {
        public ClearUserQuizProgressValidator()
        {
            RuleFor(x => x.Category)
                .NotNull().WithMessage("Loại câu hỏi không được để trống.")
                .IsInEnum().WithMessage("Loại câu hỏi không hợp lệ.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Danh mục không được để trống.");
        }
    }
}
