using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Models.NewsModel;

public class NewsResponseModel
{
    public Guid Id { get; set; }
    public Author? Author { get; set; }
    public string NewName { get; set; } = null!;
    public string? Content { get; set; }
    public string? ContentHtml { get; set; }
    public string? Image { get; set; }
    public string Slug { get; set; } = null!;
    public int? View { get; set; }
    public Guid NewsTagId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool Hot { get; set; }
    public bool IsDeleted { get; set; }
    public string? Location { get; set; }
    public string NewsTagName { get; set; } = null!;
}

public class Author
{
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorImage { get; set; }
}
