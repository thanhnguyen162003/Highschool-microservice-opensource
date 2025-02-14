using Domain.Enums;

namespace Application.Common.Models.NewsModel;

public class NewsRelatedRequestModel
{
    public Guid NewsTagId { get; set; }
    public string Location { get; set; }
}
