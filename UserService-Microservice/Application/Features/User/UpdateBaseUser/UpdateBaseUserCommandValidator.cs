using Domain.Common.Ultils;
using Domain.Constants;
using Domain.Enumerations;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.User.UpdateBaseUser
{
	public class UpdateBaseUserCommandValidator : AbstractValidator<UpdateBaseUserCommand>
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public UpdateBaseUserCommandValidator(IServiceScopeFactory serviceScopeFactory)
		{
			Configurate();
			_serviceScopeFactory = serviceScopeFactory;
		}

		public void Configurate()
		{
			RuleFor(p => p.Username)
				.Matches(RegexPattern.NotSpacePattern).WithMessage("Tên người dùng không được chứa khoảng trắng")
				.MinimumLength(4).WithMessage("Tên người dùng phải có ít nhất 4 ký tự")
				.MaximumLength(20).WithMessage("Tên người dùng không được vượt quá 20 ký tự")
				.MustAsync(IsExistUserName).WithMessage("Tên người dùng đã tồn tại");

			RuleFor(p => p.Fullname)
				.MaximumLength(100).WithMessage("Họ và tên không được vượt quá 100 chữ cái")
				.When(p => p.Fullname != null);

			RuleFor(x => x.Student.SchoolName)
				.MaximumLength(100).WithMessage("Tên trường không được vượt quá 100 ký tự")
				.When(x => x.Student != null && x.Student.SchoolName != null);

			RuleFor(x => x.Student.TypeExams)
				.ForEach(e => e.Must(EnumExtensions.IsInEnum<string, TypeExam>))
				.When(x => x.Student != null && x.Student.TypeExams != null);

			RuleFor(x => x.Student.Grade)
				.GreaterThanOrEqualTo(10).WithMessage("Lớp phải lớn hơn hoặc bằng 10")
				.LessThanOrEqualTo(12).WithMessage("Lớp phải nhỏ hơn hoặc bằng 12")
				.When(x => x.Student != null && x.Student.Grade != null);

			RuleFor(x => x.Teacher.ExperienceYears)
				.GreaterThanOrEqualTo(0).WithMessage("Số năm kinh nghiệm phải lớn hơn hoặc bằng 0")
				.When(x => x.Teacher != null && x.Teacher.ExperienceYears != null);

			RuleFor(x => x.Teacher.ContactNumber)
				.MaximumLength(11).WithMessage("Số liên lạc không được vượt quá 11 ký tự")
				.When(x => x.Teacher != null && x.Teacher.ContactNumber != null);

			RuleFor(x => x.Teacher.Pin)
				.MinimumLength(6).WithMessage("Pin phải có ít nhất 6 ký tự.")
				.MaximumLength(20).WithMessage("Pin không được vượt quá 20 ký tự")
				.When(x => x.Teacher != null && x.Teacher.Pin != null);

			RuleFor(x => x.Teacher.WorkPlace)
				.MinimumLength(5).WithMessage("Nơi làm việc phải có ít nhất 5 ký tự")
				.MaximumLength(200).WithMessage("Nơi làm việc không được vượt quá 200 ký tự")
				.When(x => x.Teacher != null && x.Teacher.WorkPlace != null);
		}


		private async Task<bool> IsExistUserName(string username, CancellationToken cancellationToken)
		{
			if (username == null) return true;

			var _httpContextAccessor = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IHttpContextAccessor>();
			var userId = _httpContextAccessor.HttpContext!.User.GetUserIdFromToken();

			var _unitOfWork = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWork>();

			return !await _unitOfWork.UserRepository.IsExistUserName(username, userId);
		}

	}
}
