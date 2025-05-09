using Domain.Entities;

namespace Application.Common.Models.UserModel
{
	public class StudentInfoResponseModel : BaseUserInforResponseModel
	{
		public int Grade { get; set; }

		public string? SchoolName { get; set; }
		public string? MbtiType { get; set; }
        public string? HollandType { get; set; }
        public string? CardUrl { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
	}
}
