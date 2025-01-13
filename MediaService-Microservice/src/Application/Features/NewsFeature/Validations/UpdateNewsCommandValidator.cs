using Application.Common.Models.NewsModel;

namespace Application.Features.NewsFeature.Validations;

public class UpdateNewsCommandValidator : AbstractValidator<NewsUpdateRequestModel>
{
    public UpdateNewsCommandValidator()
    {
        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Nội dung tin tức là bắt buộc")
            .MinimumLength(10).WithMessage("Nội dung tin tức phải có ít nhất 10 ký tự");

    }
}
//public class UpdateChapterListCommandValidator : AbstractValidator<List<DiscussUpdateRequestModel>>
//{
//    public UpdateChapterListCommandValidator()
//    {
//        RuleForEach(x => x).SetValidator(new UpdateDiscussCommandValidator());
//    }
//}
