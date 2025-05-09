namespace Application.Common.Models.UserModel
{
	public class TeacherResponse : BaseUserResponse
	{
		public string? GraduatedUniversity { get; set; }

		public string? SubjectsTaught { get; set; }
	}
}
