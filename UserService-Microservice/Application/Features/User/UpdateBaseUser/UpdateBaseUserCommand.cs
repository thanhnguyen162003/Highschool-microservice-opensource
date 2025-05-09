using Application.Common.Models.Common;
using Application.Common.Models.UserModel;

namespace Application.Features.User.UpdateBaseUser
{
	public class UpdateBaseUserCommand : IRequest<ResponseModel>
	{
		public string? Username { get; set; }

		public string? Bio { get; set; }

		public string? Fullname { get; set; }

		public string? ProfilePicture { get; set; }

		public string? Address { get; set; }

		public string? RoleName { get; set; }

		public DateTime? Birthdate { get; set; }

        public StudentRequestModel? Student { get; set; }

		public TeacherRequestModel? Teacher { get; set; }

	}
}
