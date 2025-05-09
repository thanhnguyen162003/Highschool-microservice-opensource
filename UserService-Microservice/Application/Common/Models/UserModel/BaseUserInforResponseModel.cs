using Domain.Entities;

namespace Application.Common.Models.UserModel
{
	public class BaseUserInforResponseModel
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

        public string? Address { get; set; }

        public DateTime? Birthdate { get; set; }

        public DateTime? LastLoginAt { get; set; }

		public string? ProfilePicture { get; set; }

		public DateTime? DeletedAt { get; set; }

		public string? ProgressStage { get; set; }

		public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

		public virtual ICollection<RecentView> RecentViews { get; set; } = new List<RecentView>();

		public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
	}
}
