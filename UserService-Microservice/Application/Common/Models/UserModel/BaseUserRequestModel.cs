namespace Application.Common.Models.UserModel
{
	public class BaseUserRequestModel
	{
		public string? Username { get; set; }

		public string? Bio { get; set; }

		public string? Fullname { get; set; }

		public int? RoleId { get; set; }

		public string? ProfilePicture { get; set; }

		public string? Address { get; set; }

		public StudentRequestModel? Student { get; set; }

		public TeacherRequestModel? Teacher { get; set; }
	}
}
