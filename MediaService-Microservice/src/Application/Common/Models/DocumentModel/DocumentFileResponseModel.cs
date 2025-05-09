namespace Application.Common.Models.DocumentModel;

public class DocumentFileResponseModel
{
    public Guid DocumentId { get; set; }
    public string DocumentFileUrl { get; set; }
    public string DocumentFileType { get; set; }
    public string DocumentFileExtension { get; set; }
    public string DocumentImagePreview { get; set; }
}
