namespace Application.Features.Authen.v1.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            Configurate();
        }

        private void Configurate()
        {
            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{PropertyName} là bắt buộc")
                .EmailAddress().WithMessage("{PropertyName} không phải là địa chỉ email hợp lệ.");

            RuleFor(p => p.Otp)
                .NotEmpty().WithMessage("{PropertyName} là bắt buộc")
                .Length(6).WithMessage("{PropertyName} phải là 6 ký tự");

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("{PropertyName} là bắt buộc");
        }
    }
}
