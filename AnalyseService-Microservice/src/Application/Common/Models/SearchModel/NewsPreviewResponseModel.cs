namespace Application.Common.Models.NewsModel;

public class NewsPreviewResponseModel
{
    public Guid Id { get; set; }
    public Author? Author { get; set; }
    public string NewName { get; set; } = null!;
    public string Slug { get; set; } = null!;
}

public class Author
{
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorImage { get; set; }
}
