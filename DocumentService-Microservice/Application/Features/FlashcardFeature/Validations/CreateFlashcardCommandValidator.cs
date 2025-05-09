using Application.Common.Models.FlashcardModel;
using Domain.Enums;
using Infrastructure.Constraints;

namespace Application.Features.FlashcardFeature.Validations;

public class CreateFlashcardCommandValidator : AbstractValidator<FlashcardCreateRequestModel>
{
    public CreateFlashcardCommandValidator()
    {
        RuleFor(v => v.FlashcardName)
            .NotEmpty().WithMessage("Tên thẻ ghi nhớ không được bỏ trống")
            .MaximumLength(200).WithMessage("Tên thẻ ghi nhớ không được vượt quá 200 ký tự");

        RuleFor(v => v.FlashcardDescription)
            .NotEmpty().WithMessage("Mô tả thẻ ghi nhớ không được bỏ trống")
            .MinimumLength(1).WithMessage("Mô tả thẻ ghi nhớ phải có ít nhất 1 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả thẻ ghi nhớ không được vượt quá 1000 ký tự");

        RuleFor(v => v.EntityId)
            .NotEmpty().WithMessage("ID entity không được bỏ trống");

        RuleFor(v => v.FlashcardType)
            .NotNull().WithMessage("Loại thẻ ghi nhớ không được bỏ trống")
            .IsInEnum().WithMessage("Loại thẻ ghi nhớ không hợp lệ");

        RuleFor(v => v.Status)
            .NotEmpty().WithMessage("Trạng thái thẻ ghi nhớ là bắt buộc")
            .Must(BeValidStatus).WithMessage("Trạng thái phải là một trong các trạng thái sau: OPEN,CLOSE,HIDDEN,LINK");
    }

    private bool BeValidStatus(string status)
    {
        var allowedClasses = new[] { StatusConstrains.OPEN, StatusConstrains.CLOSE, StatusConstrains.HIDDEN, StatusConstrains.ONLYLINK };
        return allowedClasses.Contains(status);
    }
}