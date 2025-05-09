namespace Application.Common.Models.NewsModel;

public class NewsAmountResponseModel
{
    public long TotalNews { get; set; }
    public long ThisMonthNews { get; set; }
    public long IncreaseNewsPercent { get; set; }
    public long TotalHotNews { get; set; }
    public long TotalDeletedNews { get; set; }
}
