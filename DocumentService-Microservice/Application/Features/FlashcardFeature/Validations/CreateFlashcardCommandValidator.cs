using Application.Common.Models.FlashcardModel;
using Infrastructure.Constraints;

namespace Application.Features.FlashcardFeature.Validations;

public class CreateFlashcardCommandValidator : AbstractValidator<FlashcardCreateRequestModel>
{
    public CreateFlashcardCommandValidator()
    {
        RuleFor(v => v.FlashcardName)
            .NotEmpty().WithMessage("Tên thẻ ghi nhớ không được bỏ trống")
            .MaximumLength(150).WithMessage("Tên thẻ ghi nhớ không được vượt quá 150 ký tự");

        RuleFor(v => v.FlashcardDescription)
            .NotEmpty().WithMessage("Mô tả thẻ ghi nhớ không được bỏ trống")
            .MinimumLength(10).WithMessage("Mô tả thẻ ghi nhớ phải có ít nhất 10 ký tự")
            .MaximumLength(255).WithMessage("Mô tả thẻ ghi nhớ không được vượt quá 255 ký tự");
        
        RuleFor(v => v.SubjectId)
            .NotEmpty().WithMessage("Môn học không được bỏ trống");

        RuleFor(v => v.Status)
            .NotEmpty().WithMessage("Trạng thái thẻ ghi nhớ là bắt buộc")
            .Must(BeValidStatus).WithMessage("Trạng thái phải là một trong các trạng thái sau: OPEN,CLOSE,HIDDEN");
    }
    private bool BeValidStatus(string status)
    {
        var allowedClasses = new[] { StatusConstrains.OPEN, StatusConstrains.CLOSE, StatusConstrains.HIDDEN, StatusConstrains.ONLYLINK };
        return allowedClasses.Contains(status);
    }
}