namespace Application.Common.Models.CurriculumModel;

public class CurriculumResponseModel
{
    public Guid? Id { get; set; }
    public string? CurriculumName { get; set; }
	public string? ImageUrl { get; set; }
	public bool IsExternal { get; set; }
}