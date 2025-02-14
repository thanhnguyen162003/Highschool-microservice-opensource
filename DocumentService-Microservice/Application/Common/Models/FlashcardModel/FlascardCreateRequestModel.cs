using System.Text.Json.Serialization;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardCreateRequestModel
{
    public string? FlashcardName { get; set; }
    
    public string? FlashcardDescription { get; set; }
    
    public string? Status { get; set; }
    
    public Guid SubjectId { get; set; }
}