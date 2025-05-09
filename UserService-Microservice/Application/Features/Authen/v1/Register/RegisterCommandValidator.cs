using Domain.Common.Ultils;
using Domain.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.Authen.v1.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RegisterCommandValidator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email là bắt buộc")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ.")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự")
                .MustAsync(BeUniqueEmail).WithMessage("Email đã tồn tại");

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Mật khẩu là bắt buộc")
                .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
                .Matches(RegexPattern.PasswordPattern).WithMessage("Mật khẩu không hợp lệ (bao gồm: chữ thường, chữ hoa, số và chữ cái đặc biệt)");

            RuleFor(v => v.FullName)
                .NotEmpty().WithMessage("Họ và tên là bắt buộc")
                .MaximumLength(100).WithMessage("Họ và tên không được vượt quá 100 ký tự");

            RuleFor(v => v.RoleName)
                .NotEmpty().WithMessage("Vai trò là bắt buộc")
                .Must(v => EnumExtensions.IsStudentOrTeacher(v)).WithMessage("Vai trò phải là Học sinh (3) hoặc Giáo viên (4)");
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var _unitOfWork = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();
            return !await _unitOfWork.UserRepository.IsExistEmail(email);
        }
    }
}
