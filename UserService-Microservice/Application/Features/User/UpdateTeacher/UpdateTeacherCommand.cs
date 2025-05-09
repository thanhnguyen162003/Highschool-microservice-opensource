using Application.Common.Models.Common;

namespace Application.Features.User.UpdateTeacher
{
	public class UpdateTeacherCommand : IRequest<ResponseModel>
	{
		public Guid BaseUserId { get; set; }

		public string? GraduatedUniversity { get; set; }

		public string? ContactNumber { get; set; }

		public string? Pin { get; set; }

		public string? WorkPlace { get; set; }

		public string? SubjectsTaught { get; set; }

		public int ExperienceYears { get; set; }
	}
}
