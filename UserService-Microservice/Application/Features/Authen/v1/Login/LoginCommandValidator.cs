namespace Application.Features.Authen.v1.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email là bắt buộc")
            .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự");

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage("Password là bắt buộc");
        }
    }
}
