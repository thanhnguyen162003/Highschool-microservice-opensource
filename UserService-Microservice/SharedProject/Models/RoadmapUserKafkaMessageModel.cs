namespace SharedProject.Models
{
	public class RoadmapUserKafkaMessageModel
	{
		public string? RoadmapId { get; set; }

		public string? RoadmapName { get; set; }

		public Guid UserId { get; set; }

		public string? ContentJson { get; set; }

		public string? RoadmapDescription { get; set; }

		public List<Guid> RoadmapSubjectIds { get; set; } = new List<Guid>();

		public List<Guid> RoadmapDocumentIds { get; set; } = new List<Guid>();

		public List<string> TypeExam { get; set; } = new List<string>();
	}
}
