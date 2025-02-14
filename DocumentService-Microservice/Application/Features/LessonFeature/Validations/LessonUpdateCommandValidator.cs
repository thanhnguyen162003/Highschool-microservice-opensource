using Application.Common.Models.LessonModel;

namespace Application.Features.LessonFeature.Validations;

public class LessonUpdateCommandValidator : AbstractValidator<LessonUpdateRequestModel>
{
    public LessonUpdateCommandValidator()
    {
        RuleFor(v => v.LessonName)
            .MinimumLength(10).WithMessage("Tên bài học phải có ít nhất 10 ký tự")
            .MaximumLength(500).WithMessage("Tên bài học không được vượt quá 500 ký tự");

        // RuleFor(v => v.LessonBody)
        //     .MinimumLength(10).WithMessage("Lesson description must at least 10 character")
        //     .MaximumLength(255).WithMessage("Lesson description must not exceed 255 characters.");
        //
        RuleFor(v => v.DisplayOrder)
            .GreaterThan(0).WithMessage("Thứ tự hiển thị phải lớn hơn 0")
            .LessThan(150).WithMessage("Thứ tự hiển thị phải nhỏ hơn 150");
    }
}
public class LessonUpdateListCommandValidator : AbstractValidator<List<LessonUpdateRequestModel>>
{
    public LessonUpdateListCommandValidator()
    {
        RuleForEach(x => x).SetValidator(new LessonUpdateCommandValidator());
    }
}