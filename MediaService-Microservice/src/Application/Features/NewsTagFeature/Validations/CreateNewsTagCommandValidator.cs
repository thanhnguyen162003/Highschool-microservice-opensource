using Application.Common.Models.NewsTagModel;

namespace Application.Features.NewsTagFeature.Validations;

public class CreateNewsTagCommandValidator : AbstractValidator<NewsTagCreateRequestModel>
{
    public CreateNewsTagCommandValidator()
    {
        RuleFor(v => v.NewTagName)
            .NotEmpty().WithMessage("Tên danh mục là bắt buộc")
            .MinimumLength(2).WithMessage("Tên danh mục phái có ít nhất 2 ký tự")
            .MaximumLength(255).WithMessage("Tên danh mục không được vượt quá 255 ký tự");
    }
}
public class CreateNewsTagListCommandValidator : AbstractValidator<List<NewsTagCreateRequestModel>>
{
    public CreateNewsTagListCommandValidator()
    {
        RuleForEach(x => x).SetValidator(new CreateNewsTagCommandValidator());
    }
}
