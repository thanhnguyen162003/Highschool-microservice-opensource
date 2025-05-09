namespace Application.Common.Models.UserModel
{
	public class StudentRequestModel
	{
		public int? Grade { get; set; }
		public string? SchoolName { get; set; }
		public string? Major { get; set; }
		public string? CardUrl { get; set; }
        public IEnumerable<string> TypeExams { get; set; } = new HashSet<string>();

		public IEnumerable<string> SubjectIds { get; set; } = new HashSet<string>();
	}
}
