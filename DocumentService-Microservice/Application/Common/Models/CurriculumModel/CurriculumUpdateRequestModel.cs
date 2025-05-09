namespace Application.Common.Models.CurriculumModel
{
	public class CurriculumUpdateRequestModel
	{
		public string? CurriculumName { get; set; }
		public required string ImageUrl { get; set; }
		public required bool IsExternal { get; set; }
	}
}
