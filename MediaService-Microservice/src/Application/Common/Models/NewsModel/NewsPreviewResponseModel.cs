using Domain.Entities.SqlEntites;

namespace Application.Common.Models.NewsModel;

public class NewsPreviewResponseModel
{
    public Guid Id { get; set; }
    public Author? Author { get; set; }
    public string NewName { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string NewsTagName { get; set; } = null!;
    public string Image { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}
