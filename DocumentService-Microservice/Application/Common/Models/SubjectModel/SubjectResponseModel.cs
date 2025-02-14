using Domain.CustomModel;

namespace Application.Common.Models.SubjectModel;

public class SubjectResponseModel
{
    public Guid Id { get; set; } 
    
    public string? SubjectName { get; set; } 
    
    public string? Image { get; set; } 
    
    public string? Information { get; set; } 
    
    public string? SubjectDescription { get; set; } 
    
    public string? SubjectCode { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public string? CategoryName { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public string Slug { get; set; } = null!;
    
    public int? Like { get; set; }
    public bool? IsLike { get; set; } = false!;

    public int? View { get; set; }
    
    public int? NumberEnrollment { get; set; }
    
    public bool IsEnroll { get; set; }

    public EnrollmentProgressModel? EnrollmentProgress { get; set; }
}