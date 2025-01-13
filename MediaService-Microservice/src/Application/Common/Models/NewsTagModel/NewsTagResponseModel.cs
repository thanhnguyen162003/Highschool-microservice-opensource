using Application.Common.Models.NewsModel;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Models.NewsTagModel;

public class NewsTagResponseModel
{
    public Guid Id { get; set; }
    public string NewTagName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
