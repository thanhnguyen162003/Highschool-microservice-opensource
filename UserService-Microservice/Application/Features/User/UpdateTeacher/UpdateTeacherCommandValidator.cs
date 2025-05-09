using Domain.Common.Ultils;
using Domain.Enumerations;

namespace Application.Features.User.UpdateTeacher
{
	public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public UpdateTeacherCommandValidator(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
			Configurate();
		}

		private void Configurate()
		{
			RuleFor(x => x.BaseUserId)
				.NotEmpty().WithMessage("UserId là bắt buộc")
				.Must(IsValidUpdate).WithMessage("Bạn không có quyền cập nhật hồ sơ này");

			RuleFor(x => x.ExperienceYears)
				.GreaterThanOrEqualTo(0).WithMessage("Số năm kinh nghiệm phải lớn hơn hoặc bằng 0");

			RuleFor(x => x.SubjectsTaught)
				.MinimumLength(5).WithMessage("Môn học giảng dạy phải có ít nhất 5 ký tự")
				.MaximumLength(100).WithMessage("Môn học giảng dạy không được vượt quá 100 ký tự");

			RuleFor(x => x.ContactNumber)
				.MaximumLength(11).WithMessage("Số liên lạc không được vượt quá 11 ký tự");

			RuleFor(x => x.Pin)
				.MinimumLength(6).WithMessage("Pin phải có ít nhất 6 ký tự")
				.MaximumLength(20).WithMessage("Pin phải không được vượt quá 20 ký tự");

			RuleFor(x => x.WorkPlace)
				.MinimumLength(5).WithMessage("Nơi làm việc phải có ít nhất 5 ký tự")
				.MaximumLength(200).WithMessage("Nơi làm việc không được vượt quá 200 ký tự");
		}

		private bool IsValidUpdate(Guid userId)
		{
			var httpContextAccessor = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IHttpContextAccessor>();

			var role = httpContextAccessor?.HttpContext!.User.GetRoleFromToken();

			if (int.Parse(role!) != (int)RoleEnum.Teacher)
			{
				return true;
			}

			var userIdFromToken = httpContextAccessor?.HttpContext!.User.GetUserIdFromToken();
			if (userIdFromToken.Equals(userId))
			{
				return true;
			}

			return false;
		}
	}
}
