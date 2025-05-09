using System.ComponentModel;

namespace Application.Common.Models.NewsModel;

public enum NewsStatisticType
{
    [Description("All")]
    All = 0,
    [Description("HotNews")]
    Hot = 1,
    [Description("DeletedNews")]
    Delete = 2
}
