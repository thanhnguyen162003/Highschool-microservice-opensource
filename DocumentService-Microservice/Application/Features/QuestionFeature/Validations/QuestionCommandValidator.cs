using Application.Common.Models.ChapterModel;
using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models.QuestionModel;
using Application.Common.UoW;
using Application.Features.ChapterFeature.Validations;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.QuestionFeature.Validations
{
    public class QuestionCommandValidator : AbstractValidator<QuestionRequestModel>
    {
        public QuestionCommandValidator()
        {

            RuleFor(v => v.CategoryId).NotEmpty().WithMessage("Danh mục không được bỏ trống");

            RuleFor(v => v.QuestionContent)
                .NotEmpty().WithMessage("Nội dung câu hỏi không được bỏ trống")
                .MinimumLength(5).WithMessage("Nội dung câu hỏi phải có ít nhất 5 ký tự")
                .MaximumLength(2000).WithMessage("Nội dung câu hỏi không được vượt quá 2000 ký tự");

            RuleFor(v => v.Category)
                .NotNull().WithMessage("Danh mục là bắt buộc")
                .IsInEnum().WithMessage("Danh mục phải là một giá trị hợp lệ");

            RuleFor(v => v.Difficulty)
                .NotNull().WithMessage("Độ khó là bắt buộc")
                .IsInEnum().WithMessage("Độ khó phải là một giá trị hợp lệ");

            RuleFor(v => v.QuestionType)
                .NotNull().WithMessage("Loại câu hỏi là bắt buộc")
                .IsInEnum().WithMessage("Loại câu hỏi phải là một giá trị hợp lệ");

            RuleFor(v => v.QuestionAnswers)
                .NotNull().WithMessage("Câu trả lời cho câu hỏi không được bỏ trống")
                .Must(x => x.Count > 0).WithMessage("Cần phải trả lời ít nhất một câu trả lời");

            RuleForEach(x => x.QuestionAnswers).ChildRules(answer =>
            {
                answer.RuleFor(x => x.AnswerContent)
                    .NotEmpty().WithMessage("Nội dung cho câu trả lời là bắt buộc")
                    .MaximumLength(500).WithMessage("Nội dung cho câu trả lời không được vượt quá 500 ký tự");

                answer.RuleFor(x => x.IsCorrectAnswer)
                    .NotNull().WithMessage("Phải chỉ định câu trả lời đúng");
            });

            RuleFor(v => v)
                .Must(QuestionTypeCheckAsync).WithMessage("Không nhất quán giữa câu trả lời và câu hỏi");
        }

        private bool QuestionTypeCheckAsync(QuestionRequestModel question)
        {
            if (question.QuestionAnswers != null && question.QuestionAnswers.Count > 0)
            {
                List<QuestionAnswerRequestModel> correctAnswerList = question.QuestionAnswers.Where(x => x.IsCorrectAnswer).ToList();
                switch (question.QuestionType)
                {
                    case QuestionType.SingleChoice:

                        if (correctAnswerList.Count == 1)
                        {
                            return true;
                        }

                        return false;

                    case QuestionType.MultipleChoice:
                        if (correctAnswerList.Count > 1)
                        {
                            return true;
                        }

                        return false;
                }

                return false;
            }
            else
            {
                return false;
            }
        }
    }
    public class QuestionListCommandValidator : AbstractValidator<List<QuestionRequestModel>>
    {
        public QuestionListCommandValidator()
        {
            RuleForEach(x => x).SetValidator(new QuestionCommandValidator());
        }
    }
}
