using Application.Common.Models.TheoryModel;

namespace Application.Features.TheoryFeature.Validator;

public class TheoryCreateCommandValidator : AbstractValidator<TheoryCreateRequestModel>
{
    public TheoryCreateCommandValidator()
    {
        RuleFor(v => v.TheoryName)
            .NotEmpty().WithMessage("Tên lý thuyết không được bỏ trống")
            .MinimumLength(1).WithMessage("Tên lý thuyết phải có ít nhất 1 ký tự")
            .MaximumLength(1000).WithMessage("Tên lý thuyết không được vượt quá 1000 ký tự");
        
        RuleFor(v => v.TheoryDescription)
            .NotEmpty().WithMessage("Mô tả lý thuyết không được bỏ trống")
            .MinimumLength(1).WithMessage("Mô tả lý thuyết phải có ít nhất 1 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả lý thuyết không được vượt quá 1000 ký tự");
        
    }
}