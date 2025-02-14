using Domain.Enums;

namespace Application.Common.Models.NewsModel;

public class NewsCreateRequestModel
{
    public Guid NewsTagId { get; set; }
    public string NewName { get; set; } = null!;
    public string? Content { get; set; }
    public string? ContentHtml { get; set; }
    public IFormFile Image { get; set; }
    public string? Location { get; set; }
}
