using Application.Common.Models.TheoryModel;

namespace Application.Features.TheoryFeature.Validator;

public class TheoryUpdateCommandValidator : AbstractValidator<TheoryUpdateRequestModel>
{
    public TheoryUpdateCommandValidator()
    {
        RuleFor(v => v.TheoryName)
            .MinimumLength(10).WithMessage("Tên lý thuyết phải có ít nhất 10 ký tự")
            .MaximumLength(1000).WithMessage("Tên lý thuyết không được vượt quá 1000 ký tự");

        RuleFor(v => v.TheoryDescription)  
            .MinimumLength(10).WithMessage("Mô tả lý thuyết phải có ít nhất 10 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả lý thuyết không được vượt quá 1000 ký tự");

    }
}