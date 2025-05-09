using Application.Common.Models.Common;

namespace Application.Features.User.UpdateStudent
{
	public class UpdateStudentCommand : IRequest<ResponseModel>
	{
		public Guid BaseUserId { get; set; }

		public int Grade { get; set; }

		public string? SchoolName { get; set; }

		public string? Major { get; set; }
		public string? CardUrl { get; set; }

		public IEnumerable<string> TypeExams { get; set; } = new HashSet<string>();

		public IEnumerable<string> SubjectIds { get; set; } = new HashSet<string>();
	}
}
