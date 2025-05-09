using Application.Common.Models.LessonModel;

namespace Application.Features.LessonFeature.Validations;

public class LessonCreateCommandValidator : AbstractValidator<LessonCreateRequestModel>
{
    public LessonCreateCommandValidator()
    {
        RuleFor(v => v.LessonName)
            .NotEmpty().WithMessage("Tên bài học không được bỏ trống")
            .MinimumLength(1).WithMessage("Tên bài học phải có ít nhất 1 ký tự")
            .MaximumLength(200).WithMessage("Tên bài học không được vượt quá 200 ký tự");
        
        // RuleFor(v => v.LessonBody)
        //     .MinimumLength(10).WithMessage("Lesson description must at least 10 character")
        //     .MaximumLength(500).WithMessage("Lesson description must not exceed 500 characters.");
        //
        RuleFor(v => v.DisplayOrder)
            .NotEmpty().WithMessage("Thứ tự hiển thị là bắt buộc")
            .GreaterThan(0).WithMessage("Thứ tự hiển thị phải lớn hơn 0")
            .LessThan(150).WithMessage("Thứ tự hiển thị phải nhỏ hơn 150");
    }
}
public class LessonCreateListCommandValidator : AbstractValidator<List<LessonCreateRequestModel>>
{
    public LessonCreateListCommandValidator()
    {
        RuleForEach(x => x).SetValidator(new LessonCreateCommandValidator());
    }
}