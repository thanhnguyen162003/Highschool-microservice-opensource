namespace SharedProject.Models;

public class SubjectImageCreateModel
{
    public Guid SubjectId { get; set; }
    public string? ImageUrl { get; set; }
    public string? PublicIdUrl { get; set; }
    public string? Format { get; set; }
}
