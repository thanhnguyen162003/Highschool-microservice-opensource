using System.Text.Json.Serialization;

namespace Application.Common.Models.SubjectModel;

public class SubjectCreateRequestModel
{
    public string? SubjectName { get; set; }
    
    public IFormFile? ImageRaw { get; set; }

    public Guid? CategoryId { get; set; }
    
    public string? SubjectDescription { get; set; } 
    
    public string? SubjectCode { get; set; }
    
    public string? Information { get; set; } 
    
}