namespace Application.Common.Models.TheoryModel;

public class TheoryFileUploadRequestModel
{
    public string? FileName { get; set; }
    public List<IFormFile>? ImageFiles { get; set; }
    public List<IFormFile>? FileDocument { get; set; }
}
