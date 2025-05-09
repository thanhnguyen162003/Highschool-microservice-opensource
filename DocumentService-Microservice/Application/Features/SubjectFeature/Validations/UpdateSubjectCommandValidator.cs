using Application.Features.SubjectFeature.Commands;
using Domain.CustomModel;

namespace Application.Features.SubjectFeature.Validations;

public class UpdateSubjectCommandValidator : AbstractValidator<SubjectModel>
{
    public UpdateSubjectCommandValidator()
    {
        RuleFor(v => v.SubjectName)
            .MinimumLength(1).WithMessage("Tên môn học phải có ít nhất 1 ký tự")
            .MaximumLength(200).WithMessage("Tên môn học không được vượt quá 200 ký tự");

        RuleFor(v => v.SubjectCode)
            .MinimumLength(3).WithMessage("Mã môn học phải có ít nhất 3 ký tự")
            .MaximumLength(100).WithMessage("Mã môn học không được vượt quá 100 ký tự");

        RuleFor(v => v.SubjectDescription)
            .MinimumLength(1).WithMessage("Mô tả môn học phải có ít nhất 1 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả môn học không được vượt quá 1000 ký tự");

        RuleFor(v => v.Information)
            .MinimumLength(1).WithMessage("Thông tin môn học phải có ít nhất 1 ký tự")
            .MaximumLength(1000).WithMessage("Thông tin môn học không được vượt quá 1000 ký tự");

        RuleFor(v => v.Category)
            .Must(category => category == null || BeAValidClass(category))
            .WithMessage("Class must be one of: Grade10, 11, 12");
    }
    private bool BeAValidClass(string? @class)
    {
		var allowedClasses = new[] { "Grade10", "Grade11", "Grade12", "THPTQG", "DGNL" };
		return allowedClasses.Contains(@class);
	}
}