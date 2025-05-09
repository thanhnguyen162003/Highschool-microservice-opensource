using Domain.Enums;

namespace Application.Common.Models.NewsModel;

public class NewsCreateRequestModel
{
    public Guid NewsTagId { get; set; }
    public List<Guid>? FlashcardIds { get; set; }
    public List<Guid>? DocumentIds { get; set; }
    //public List<Guid>? TheoryIds { get; set; }
    public string NewName { get; set; } = null!;
    public string? Content { get; set; }
    public string? ContentHtml { get; set; }
    public string Image { get; set; }
    public string? Location { get; set; }
}
