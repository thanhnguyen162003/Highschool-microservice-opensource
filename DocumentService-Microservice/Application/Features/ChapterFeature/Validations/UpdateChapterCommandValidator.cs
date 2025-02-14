using Application.Common.Models.ChapterModel;

namespace Application.Features.ChapterFeature.Validations;

public class UpdateChapterCommandValidator : AbstractValidator<ChapterUpdateRequestModel>
{
    public UpdateChapterCommandValidator()
    {
        RuleFor(v => v.ChapterName)
            .MinimumLength(10).WithMessage("Tên của chương học phải có ít nhất 10 ký tự")
            .MaximumLength(1000).WithMessage("Tên của chương học không được vượt quá 1000 ký tự");

        RuleFor(v => v.Description)
            .MinimumLength(10).WithMessage("Mô tả của chương học phải có ít nhất 10 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả của chương học không được vượt quá 1000 ký tự");

        RuleFor(v => v.ChapterLevel)
            .GreaterThan(0).WithMessage("Cấp độ chương phải trên 0")
            .LessThan(150).WithMessage("Cấp độ chương phải nhỏ hơn 150");
    }
}
public class UpdateChapterListCommandValidator : AbstractValidator<List<ChapterUpdateRequestModel>>
{
    public UpdateChapterListCommandValidator()
    {
        RuleForEach(x => x).SetValidator(new UpdateChapterCommandValidator());
    }
}