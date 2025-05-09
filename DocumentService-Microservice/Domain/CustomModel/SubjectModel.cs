using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Domain.CustomModel;

public class SubjectModel
{
    public Guid Id { get; set; } 
    public string? SubjectName { get; set; } 
    public string? Image { get; set; }
    public IFormFile? ImageRaw { get; set; }
    public string? SubjectDescription { get; set; } 
    public string? Information { get; set; } 
    public string? SubjectCode { get; set; }
    public string? Category { get; set; }
    public Guid? MasterSubjectId { get; set; }
    public string? MasterSubjectName { get; set; }
	public string? MasterSubjectSlug { get; set; }
	public string? Class { get; set; }
	public Guid? CurriculumIdUser { get; set; }
	public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Slug { get; set; }
    public int? Like { get; set; }
    public int? View { get; set; }
    public bool? IsLike { get; set; } = false!;
    public int? NumberEnrollment { get; set; }
    public bool IsEnroll { get; set; }
    public EnrollmentProgressModel? EnrollmentProgress { get; set; }
}