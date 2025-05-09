namespace Domain.Entities
{
	public class UserSubject
	{
		public Guid UserId { get; set; }
		public Guid? SubjectId { get; set; }

		public virtual BaseUser User { get; set; } = null!;
	}
}
