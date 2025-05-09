using Application.Common.Models.FSRSPresetModel;
using Application.Common.Models.LessonModel;

namespace Application.Features.FSRSPresetFeature.Validations;

public class FSRSPresetCreateCommandValidator : AbstractValidator<FSRSPresetCreateRequest>
{
    public FSRSPresetCreateCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title không được bỏ trống")
            .MinimumLength(1).WithMessage("Title phải có ít nhất 1 ký tự")
            .MaximumLength(200).WithMessage("Title không được vượt quá 200 ký tự");

        RuleFor(v => v.Retrievability)
            .NotEmpty().WithMessage("Retrievability là bắt buộc")
            .GreaterThan(0).WithMessage("Retrievability phải lớn hơn 0")
            .LessThan(1).WithMessage("Retrievability phải nhỏ hơn 1");
        RuleFor(v => v.FsrsParameters)
            .NotEmpty().WithMessage("FsrsParameters là bắt buộc")
            .Must(p => p != null && p.Length == 19)
            .WithMessage("FsrsParameters phải chứa đúng 19 phần tử")
            .Must(p => p.All(value => value > 0))
            .WithMessage("Mỗi phần tử trong FsrsParameters phải lớn hơn 0");
    }
}
