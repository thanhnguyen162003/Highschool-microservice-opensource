using Infrastructure.Repositories.Interfaces;

namespace Application.Features.Authen.v1.AuthenWithGoogle.RegisterWithGoogle
{
    public class RegisterWithGoogleCommandValiator : AbstractValidator<RegisterWithGoogleCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RegisterWithGoogleCommandValiator(IServiceScopeFactory serviceProviderFactory)
        {
            ConfigureValidationRules();
            _serviceScopeFactory = serviceProviderFactory;
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email là bắt buộc")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự")
                .MustAsync(BeUniqueEmail).WithMessage("Email đã tồn tại");

            RuleFor(v => v.FullName)
                .NotEmpty().WithMessage("Họ và tên là bắt buộc")
                .MaximumLength(100).WithMessage("Tên đầy đủ không được vượt quá 100 ký tự");
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var _unitOfWork = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();
            return !await _unitOfWork.UserRepository.IsExistEmail(email);
        }
    }
}
