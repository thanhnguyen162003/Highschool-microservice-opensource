using Application.Common.Models.NewsModel;

namespace Application.Features.NewsFeature.Validations;

public class CreateNewsCommandValidator : AbstractValidator<NewsCreateRequestModel>
{
    public CreateNewsCommandValidator()
    {
        RuleFor(v => v.NewName)
            .NotEmpty().WithMessage("Tiêu đề tin tức là bắt buộc")
            .MinimumLength(10).WithMessage("Tiêu đề tin tức phải có ít nhất 10 ký tự")
            .MaximumLength(255).WithMessage("Tiêu đề tin tức không được vượt quá 255 ký tự");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Nội dung tin tức là bắt buộc")
            .MinimumLength(10).WithMessage("Nội dung tin tức phải có ít nhất 10 ký tự");


    }
}
//public class CreateChapterListCommandValidator : AbstractValidator<List<DiscussCreateRequestModel>>
//{
//    public CreateChapterListCommandValidator()
//    {
//        RuleForEach(x => x).SetValidator(new CreateChapterCommandValidator());
//    }
//}
