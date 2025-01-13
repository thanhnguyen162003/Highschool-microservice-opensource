using Application.Common.Models.ChapterModel;

namespace Application.Features.ChapterFeature.Validations;

public class CreateChapterCommandValidator : AbstractValidator<ChapterCreateRequestModel>
{
    public CreateChapterCommandValidator()
    {
        RuleFor(v => v.ChapterName)
            .NotEmpty().WithMessage("Tên của chương học không được bỏ trống")
            .MinimumLength(10).WithMessage("Tên của chương học phải có ít nhất 10 ký tự")
            .MaximumLength(1000).WithMessage("Tên của chương học không được vượt quá 1000 ký tự");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Mô tả của chương học không được bỏ trống")
            .MinimumLength(10).WithMessage("Mô tả của chương học phải có ít nhất 10 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả của chương học không được vượt quá 1000 ký tự");

        RuleFor(v => v.ChapterLevel)
            .NotEmpty().WithMessage("Cấp độ chương học không được bỏ trống")
            .GreaterThan(0).WithMessage("Cấp độ chương phải trên 0")
            .LessThan(150).WithMessage("Cấp độ chương phải nhỏ hơn 150");
    }
}
public class CreateChapterListCommandValidator : AbstractValidator<List<ChapterCreateRequestModel>>
{
    public CreateChapterListCommandValidator()
    {
        RuleForEach(x => x).SetValidator(new CreateChapterCommandValidator());
    }
}