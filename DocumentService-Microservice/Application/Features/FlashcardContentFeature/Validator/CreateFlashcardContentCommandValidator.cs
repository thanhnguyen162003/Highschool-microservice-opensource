using Application.Common.Models.FlashcardContentModel;
using Infrastructure.Constraints;

namespace Application.Features.FlashcardContentFeature.Validator;

public class CreateFlashcardContentCommandValidator : AbstractValidator<FlashcardContentCreateRequestModel>
{
    public CreateFlashcardContentCommandValidator()
    {
        RuleFor(v => v.FlashcardContentTerm)
            .NotEmpty().WithMessage("Thuật ngữ không được bỏ trống")
            .MaximumLength(1000).WithMessage("Thuật ngữ không được vượt quá 1000 ký tự");

        RuleFor(v => v.FlashcardContentDefinition)
            .NotEmpty().WithMessage("Định nghĩa không được bỏ trống")
            .MaximumLength(1000).WithMessage("Định nghĩa không được vượt quá 1000 ký tự");
    }
}
public class CreateFlashcardContentListValidator : AbstractValidator<List<FlashcardContentCreateRequestModel>>
{
    public CreateFlashcardContentListValidator()
    {
        RuleForEach(x => x).SetValidator(new CreateFlashcardContentCommandValidator());
    }
}