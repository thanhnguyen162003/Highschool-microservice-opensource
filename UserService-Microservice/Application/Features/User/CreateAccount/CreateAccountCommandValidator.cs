using Domain.Common.Ultils;
using Domain.Constants;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.User.CreateAccount
{
	public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public CreateAccountCommandValidator(IServiceScopeFactory serviceScopeFactory)
		{
			Configurate();
			_serviceScopeFactory = serviceScopeFactory;
		}

		private void Configurate()
		{
			RuleFor(p => p.Username)
				.Matches(RegexPattern.NotSpacePattern).WithMessage("Tên người dùng không được chứa khoảng trắng")
				.MinimumLength(4).WithMessage("Tên người dùng phải có ít nhất 4 ký tự")
				.MaximumLength(20).WithMessage("Tên người dùng không được vượt quá 20 ký tự")
				.MustAsync(IsExistUserName).WithMessage("Tên người dùng đã tồn tại");

			RuleFor(v => v.Email)
				.NotEmpty().WithMessage("Email là bắt buộc")
				.EmailAddress().WithMessage("Định dạng email không hợp lệ")
				.MaximumLength(100).WithMessage("Email không được quá 100 ký tự")
				.MustAsync(BeUniqueEmail).WithMessage("Email đã tồn tại");

			RuleFor(v => v.Password)
				.NotEmpty().WithMessage("Mật khẩu là bắt buộc")
				.MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
				.Matches(RegexPattern.PasswordPattern).WithMessage("Mật khẩu không hợp lệ (bao gồm: chữ thường, chữ hoa, số và chữ cái đặc biệt)");

			RuleFor(v => v.Fullname)
				.NotEmpty().WithMessage("Họ và tên là bắt buộc")
				.MaximumLength(100).WithMessage("Họ và tên không được vượt quá 100 ký tự");

			RuleFor(v => v.ProfilePicture)
				.MaximumLength(10000).WithMessage("Ảnh đại diện không được vượt quá 10000 ký tự");
		}

		private async Task<bool> IsExistUserName(string username, CancellationToken cancellationToken)
		{
			if (username == null) return true; // if username is null, it is not exist

			var _httpContextAccessor = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IHttpContextAccessor>();
			var userId = _httpContextAccessor.HttpContext!.User.GetUserIdFromToken();

			var _unitOfWork = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();

			return !await _unitOfWork.UserRepository.IsExistUserName(username, userId);
		}

		private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
		{
			var _unitOfWork = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();
			return !await _unitOfWork.UserRepository.IsExistEmail(email);
		}


	}
}
