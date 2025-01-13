using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Common.Models.NewsModel;

public class NewsUpdateRequestModel
{
    [Required]
    public Guid NewsTagId { get; set; }
    [Required]
    public string NewName { get; set; } = null!;
    [Required]
    public string? Content { get; set; }
    public string? ContentHtml { get; set; }
    [Required]
    public string? Location { get; set; }
    public bool Hot { get; set; }
}
