namespace Application.Common.Models.TheoryModel;

public record TheoryUpdateRequestModel
{
    public string? TheoryName { get; set; }

    public string? TheoryDescription { get; set; }

    public string? TheoryContentJson { get; set; }

    public string? TheoryContentHtml { get; set; }
}