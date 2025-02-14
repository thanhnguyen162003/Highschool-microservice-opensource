namespace Application.Common.Models.NewsModel;

public class NewsFileUploadRequestModel
{
    public string? FileName { get; set; }
    public IFormFile ImageFiles { get; set; }
}
