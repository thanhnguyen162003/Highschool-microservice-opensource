using Application.Common.Models.QuestionAnswerModel;
using Domain;

namespace Application.Features.QuestionFeature.Validations
{
    public class SubmitAnswerRequestValidator : AbstractValidator<SubmitAnswerRequestModel>
    {
        public SubmitAnswerRequestValidator()
        {
            RuleFor(x => x.QuestionCategory)
                .NotNull().WithMessage("Loại câu hỏi không được để trống.")
                .IsInEnum().WithMessage("Loại câu hỏi không hợp lệ.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Danh mục không được để trống.");

            RuleFor(x => x.Answers)
                .NotNull().WithMessage("Danh sách câu trả lời không được để trống.");
                //.Must((model, answers) =>
                //{
                //    var expectedCount = GlobalConstant.NumberOfQuestionsFromCategory.GetValueOrDefault(model.QuestionCategory, 0);
                //    return answers != null && (answers.Count == expectedCount || answers.Count == expectedCount / 2);
                //}).WithMessage(x => $"Danh sách câu trả lời phải có số lượng đúng với danh mục {x.QuestionCategory}.");

            RuleForEach(x => x.Answers).ChildRules(answer =>
            {
                answer.RuleFor(a => a.QuestionId)
                    .NotEmpty().WithMessage("Câu hỏi không được để trống.");

                answer.RuleFor(a => a.QuestionAnswerIds)
                    .NotNull().WithMessage("Danh sách đáp án không được để trống.")
                    .Must(ids => ids.Count > 0).WithMessage("Danh sách đáp án phải có ít nhất một phần tử.");
            });
        }
    }
}
