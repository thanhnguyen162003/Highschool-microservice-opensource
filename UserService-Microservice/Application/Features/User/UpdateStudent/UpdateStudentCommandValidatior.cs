using Domain.Common.Ultils;
using Domain.Enumerations;

namespace Application.Features.User.UpdateStudent
{
	public class UpdateStudentCommandValidatior : AbstractValidator<UpdateStudentCommand>
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public UpdateStudentCommandValidatior(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
			Configurate();
		}

		private void Configurate()
		{
			RuleFor(x => x.BaseUserId)
				.NotEmpty().WithMessage("UserId là bắt buộc")
				.Must(IsValidUpdate).WithMessage("Bạn không có quyền cập nhật hồ sơ này");

			RuleFor(x => x.SchoolName)
				.MaximumLength(100).WithMessage("Tên trường không được vượt quá 100 ký tự");

			RuleFor(x => x.TypeExams)
				.ForEach(e => e.Must(EnumExtensions.IsInEnum<TypeExam, string>));
		}


		private bool IsValidUpdate(Guid userId)
		{
			var httpContextAccessor = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IHttpContextAccessor>();

			var role = httpContextAccessor?.HttpContext!.User.GetRoleFromToken();

			if (int.Parse(role!) != (int)RoleEnum.Student)
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
