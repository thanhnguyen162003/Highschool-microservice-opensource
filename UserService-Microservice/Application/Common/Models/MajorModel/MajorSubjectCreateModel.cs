namespace Application.Common.Models.MajorModel
{
	public class MajorSubjectCreateModel
	{
		public required string MajorId { get; set; }
		public List<Guid>? MasterSubjectIds { get; set; }
	}
}
