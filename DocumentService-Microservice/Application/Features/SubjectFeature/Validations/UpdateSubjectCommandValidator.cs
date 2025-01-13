using Application.Features.SubjectFeature.Commands;
using Domain.CustomModel;

namespace Application.Features.SubjectFeature.Validations;

public class UpdateSubjectCommandValidator : AbstractValidator<SubjectModel>
{
    public UpdateSubjectCommandValidator()
    {
        RuleFor(v => v.SubjectName)
            .MinimumLength(10).WithMessage("Tên môn học phải có ít nhất 10 ký tự")
            .MaximumLength(1000).WithMessage("Tên môn học không được vượt quá 1000 ký tự");

        RuleFor(v => v.SubjectCode)
            .MinimumLength(3).WithMessage("Mã môn học phải có ít nhất 3 ký tự")
            .MaximumLength(100).WithMessage("Mã môn học không được vượt quá 100 ký tự");

        RuleFor(v => v.SubjectDescription)
            .MinimumLength(20).WithMessage("Mô tả môn học phải có ít nhất 20 ký tự")
            .MaximumLength(1000).WithMessage("Mô tả môn học không được vượt quá 1000 ký tự");

        RuleFor(v => v.Information)
            .MinimumLength(20).WithMessage("Thông tin môn học phải có ít nhất 20 ký tự")
            .MaximumLength(1000).WithMessage("Thông tin môn học không được vượt quá 1000 ký tự");

        // RuleFor(v => v.Class)
        //     .Must(BeAValidClass).WithMessage("Class must be one of: 10, 11, 12, THPTQG, DGNL, IN10CLASS");
    }
    private bool BeAValidClass(string Class)
    {
        var allowedClasses = new[] { "10", "11", "12", "THPTQG","DGNL", "IN10CLASS"};
        return allowedClasses.Contains(Class);
    }
}