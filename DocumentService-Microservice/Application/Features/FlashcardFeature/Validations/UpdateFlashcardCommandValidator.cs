using Application.Common.Models.FlashcardModel;
using Domain.Enums;
using Infrastructure.Constraints;

namespace Application.Features.FlashcardFeature.Validations;

public class UpdateFlashcardCommandValidator : AbstractValidator<FlashcardUpdateRequestModel>
{
    public UpdateFlashcardCommandValidator()
    {
        RuleFor(v => v.FlashcardName)
            .MinimumLength(1).WithMessage("Tên thẻ ghi nhớ phải có ít nhất 1 ký tự")
            .MaximumLength(200).WithMessage("Tên thẻ ghi nhớ không được vượt quá 200 ký tự");

        RuleFor(v => v.FlashcardDescription)
            .MinimumLength(1).WithMessage("Mô tả thẻ ghi nhớ phải có ít nhất 1 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả thẻ ghi nhớ không được vượt quá 1000 ký tự");
            
        // EntityId và FlashcardType phải tồn tại cùng nhau
        When(v => v.EntityId.HasValue, () => {
            RuleFor(v => v.FlashcardType)
                .NotNull().WithMessage("Khi cung cấp EntityId, FlashcardType không được để trống");
        });

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