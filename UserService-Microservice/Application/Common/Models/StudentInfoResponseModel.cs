using Application.Common.Models.UserModel;
using Domain.Entities;

namespace Application.Common.Models
{
	public class StudentInfoResponseModel : BaseUserInforResponseModel
	{
		public int Grade { get; set; }

		public string? SchoolName { get; set; }
		public string? MbtiType { get; set; }
		public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();  
	}
}
