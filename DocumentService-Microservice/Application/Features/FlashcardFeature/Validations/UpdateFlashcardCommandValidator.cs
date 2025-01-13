using Application.Common.Models.FlashcardModel;
using Infrastructure.Constraints;

namespace Application.Features.FlashcardFeature.Validations;

public class UpdateFlashcardCommandValidator : AbstractValidator<FlashcardUpdateRequestModel>
{
    public UpdateFlashcardCommandValidator()
    {
        RuleFor(v => v.FlashcardName)
            .MinimumLength(1).WithMessage("Tên thẻ ghi nhớ phải có ít nhất 10 ký tự")
            .MaximumLength(150).WithMessage("Tên thẻ ghi nhớ không được vượt quá 150 ký tự");

        RuleFor(v => v.FlashcardDescription)
            .MinimumLength(10).WithMessage("Mô tả thẻ ghi nhớ phải có ít nhất 10 ký tự")
            .MaximumLength(255).WithMessage("Mô tả thẻ ghi nhớ không được vượt quá 255 ký tự");

        RuleFor(v => v.Status)
            .NotEmpty().When(v => !string.IsNullOrEmpty(v.Status))
            .Must(BeValidStatus).When(v => !string.IsNullOrEmpty(v.Status))
            .WithMessage("Trạng thái phải là một trong các trạng thái sau: OPEN, CLOSE, HIDDEN, ONLYLINK");
    }
    private bool BeValidStatus(string status)
    {
        var allowedClasses = new[] { StatusConstrains.OPEN, StatusConstrains.CLOSE, StatusConstrains.HIDDEN, StatusConstrains.ONLYLINK };
        return allowedClasses.Contains(status);
    }
}