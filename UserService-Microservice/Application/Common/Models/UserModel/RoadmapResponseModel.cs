using Domain.Entities;

namespace Application.Common.Models.UserModel
{
	public class RoadmapResponseModel
	{
		public string Id { get; set; }

		public Guid UserId { get; set; }

		public string? Name { get; set; }

		public string? Description { get; set; }

		public string? ContentJson { get; set; }

		public List<Guid> SubjectIds { get; set; } = new List<Guid>();

		public List<Guid> DocumentIds { get; set; } = new List<Guid>();

		public List<string> TypeExam { get; set; } = new List<string>();

		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }
	}
}
