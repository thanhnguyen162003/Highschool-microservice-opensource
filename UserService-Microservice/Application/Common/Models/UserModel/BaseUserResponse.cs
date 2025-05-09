namespace Application.Common.Models.UserModel
{
	public class BaseUserResponse
	{
		public Guid Id { get; set; }

		public string? Username { get; set; }

		public string? Email { get; set; } = null!;

		public string? Fullname { get; set; }

		public string? RoleName { get; set; }

		public string? Status { get; set; }

		public string? ProfilePicture { get; set; }

		public DateTime? CreatedAt { get; set; }
	}
}
