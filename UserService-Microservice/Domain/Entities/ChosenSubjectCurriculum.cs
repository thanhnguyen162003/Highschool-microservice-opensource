using Domain.Common;

namespace Domain.Entities
{
    public class ChosenSubjectCurriculum : BaseAuditableEntity
	{
		public Guid Id { get; set; }
		public Guid SubjectId { get; set; }
		public Guid CurriculumId { get; set; }
		public Guid? SubjectCurriculumId { get; set; }
		public Guid UserId { get; set; }
	}
}
