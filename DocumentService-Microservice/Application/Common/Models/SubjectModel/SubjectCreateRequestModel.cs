using Domain.Enums;

namespace Application.Common.Models.SubjectModel;

public class SubjectCreateRequestModel
{
    public string? SubjectName { get; set; }
    
    public IFormFile? ImageRaw { get; set; }

    public Guid MasterSubjectId { get; set; }

    public string? Category { get; set; }
    
    public string? SubjectDescription { get; set; } 
    
    public string? SubjectCode { get; set; }
    
    public string? Information { get; set; }

	public bool? IsExternal { get; set; }
}