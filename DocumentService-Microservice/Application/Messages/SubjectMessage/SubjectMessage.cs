namespace Application.Messages.SubjectMessage
{
	public class SubjectMessage
	{
		public Guid SubjectId { get; set; }
		public required string MasterSubjectName { get; set; }
		public required string MasterSubjectSlug { get; set; }
	}
}
