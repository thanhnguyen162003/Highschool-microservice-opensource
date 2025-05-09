using Application.Common.Models.CurriculumModel;

namespace Application.Features.CurrilumFeature.Validatation;

public class CurriculumCreateValidation : AbstractValidator<CurriculumCreateRequestModel>
{
    public CurriculumCreateValidation()
    {
        RuleFor(v => v.CurriculumName)
            .NotEmpty().WithMessage("Tên của chương trình học không được bỏ trống")
            .MaximumLength(200).WithMessage("Tên của chương trình học không được vượt quá 200 ký tự");
        
    }
}