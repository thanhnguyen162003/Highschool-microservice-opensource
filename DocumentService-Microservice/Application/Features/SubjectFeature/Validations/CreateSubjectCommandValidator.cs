using Application.Common.Models.SubjectModel;
using Application.Features.SubjectFeature.Commands;

namespace Application.Features.SubjectFeature.Validations;

public class CreateSubjectCommandValidator : AbstractValidator<SubjectCreateRequestModel>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(v => v.SubjectName)
            .NotEmpty().WithMessage("Tên môn học không được bỏ trống")
            .MinimumLength(1).WithMessage("Tên môn học phải có ít nhất 1 ký tự")
            .MaximumLength(200).WithMessage("Tên môn học không được vượt quá 200 ký tự");
        
        RuleFor(v => v.SubjectCode)
            .NotEmpty().WithMessage("Mã môn học là bắt buộc")
            .MinimumLength(3).WithMessage("Mã môn học phải có ít nhất 3 ký tự")
            .MaximumLength(100).WithMessage("Mã môn học không được vượt quá 100 ký tự");

        RuleFor(v => v.SubjectDescription)
            .NotEmpty().WithMessage("Mô tả môn học không được bỏ trống")
            .MinimumLength(1).WithMessage("Mô tả môn học phải có ít nhất 20 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả môn học không được vượt quá 1000 ký tự");

        // RuleFor(v => v.ImageRaw)
        //     .NotEmpty().WithMessage("Subject image is required.");
        
        RuleFor(v => v.Information)
            .NotEmpty().WithMessage("Thông tin môn học không được bỏ trống")
            .MinimumLength(1).WithMessage("Thông tin môn học phải có ít nhất 1 ký tự")
            .MaximumLength(1000).WithMessage("Thông tin môn học không được vượt quá 1000 ký tự");

        RuleFor(v => v.Category)
            .NotEmpty().WithMessage("Subject class is required.")
            .Must(BeAValidClass).WithMessage("Class must be one of: Grade10, 11, 12");
    }
    private bool BeAValidClass(string? @class)
    {
        var allowedClasses = new[] { "Grade10", "Grade11", "Grade12", "THPTQG", "DGNL" };
        return allowedClasses.Contains(@class);
    }
}