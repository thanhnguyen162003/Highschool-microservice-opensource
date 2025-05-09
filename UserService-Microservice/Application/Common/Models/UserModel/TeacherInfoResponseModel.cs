using Domain.Entities;

namespace Application.Common.Models.UserModel
{
	public class TeacherInfoResponseModel : BaseUserInforResponseModel
	{
		public string? GraduatedUniversity { get; set; }

		public string? ContactNumber { get; set; }

		public string? Pin { get; set; }

		public string? WorkPlace { get; set; }

		public string? SubjectsTaught { get; set; }

		public double? Rating { get; set; }

		public int ExperienceYears { get; set; }

		public bool Verified { get; set; }

		public string? VideoIntroduction { get; set; }

		public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
	}
}
