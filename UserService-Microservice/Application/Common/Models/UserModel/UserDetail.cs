namespace Application.Common.Models.UserModel
{
	public class UserDetail
	{
		public Guid Id { get; set; }

		public string? Username { get; set; }

		public string? Email { get; set; } = null!;

		public string? Bio { get; set; }

		public string? Fullname { get; set; }

		public string? RoleName { get; set; }

		public string? Provider { get; set; }

		public string? Status { get; set; }

		public string? Timezone { get; set; }

		public DateTime? LastLoginAt { get; set; }

		public string? ProfilePicture { get; set; }

		public DateTime? DeletedAt { get; set; }

		public string? ProgressStage { get; set; }
	}
}
