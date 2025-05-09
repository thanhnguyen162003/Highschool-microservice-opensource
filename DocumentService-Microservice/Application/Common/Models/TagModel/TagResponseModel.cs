namespace Application.Common.Models.TagModel;

public class TagResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}

public class TagCreateRequestModel
{
    public List<string> Name { get; set; } = new List<string>();
}

public class TagUpdateRequestModel
{
    public string Name { get; set; } = string.Empty;
}