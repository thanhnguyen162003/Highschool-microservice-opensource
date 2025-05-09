using Application.Common.Models.FlashcardContentModel;

namespace Application.Features.FlashcardContentFeature.Validator;

public class FlashcardContentUpdateRequestModelValidator : AbstractValidator<FlashcardContentUpdateRequestModel>
{
    public FlashcardContentUpdateRequestModelValidator()
    {
        RuleFor(v => v.FlashcardContentTerm)
            .MaximumLength(1000).WithMessage("Thuật ngữ không được vượt quá 1000 ký tự");

        RuleFor(v => v.FlashcardContentDefinition)
            .MaximumLength(1000).WithMessage("Định nghĩa không được vượt quá 1000 ký tự");
    }
}

public class UpdateFlashcardContentListValidator : AbstractValidator<List<FlashcardContentUpdateRequestModel>>
{
    public UpdateFlashcardContentListValidator()
    {
        RuleForEach(x => x).SetValidator(new FlashcardContentUpdateRequestModelValidator());
    }
}
