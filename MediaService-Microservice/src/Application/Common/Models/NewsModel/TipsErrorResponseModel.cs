using Domain.Enums;

namespace Application.Common.Models.NewsModel;

public class TipsErrorResponseModel
{
    public string Flashcard { get; set; }
    public string Document { get; set; }
    public string Theory { get; set; }
    public bool IsError { get; set; }

}
