using Application.Common.Models.NewsTagModel;

namespace Application.Features.NewsTagFeature.Validations;

public class UpdateNewsTagCommandValidator : AbstractValidator<NewsTagUpdateRequestModel>
{
    public UpdateNewsTagCommandValidator()
    {
        RuleFor(v => v.NewTagName)
            .NotEmpty().WithMessage("Tên danh mục là bắt buộc")
            .MinimumLength(2).WithMessage("Tên danh mục phái có ít nhất 2 ký tự")
            .MaximumLength(255).WithMessage("Tên danh mục không được vượt quá 255 ký tự");
    }
}
//public class UpdateChapterListCommandValidator : AbstractValidator<List<DiscussUpdateRequestModel>>
//{
//    public UpdateChapterListCommandValidator()
//    {
//        RuleForEach(x => x).SetValidator(new UpdateDiscussCommandValidator());
//    }
//}
