using Domain.Common;

namespace Domain.Entities
{
	public class Roadmap : BaseAuditableEntity
	{
		public string Id { get; set; }

		public Guid UserId { get; set; }

		public string? Name { get; set; }

		public string? Description { get; set; }

		public string? ContentJson { get; set; }

		public string? SubjectId { get; set; }

		public string? DocumentId { get; set; }

		public string? TypeExam { get; set; }

		public bool IsDeleted { get; set; } = false;

		public virtual BaseUser? User { get; set; }
	}
}
