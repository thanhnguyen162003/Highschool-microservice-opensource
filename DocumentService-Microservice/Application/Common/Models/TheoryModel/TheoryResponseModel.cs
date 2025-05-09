namespace Application.Common.Models.TheoryModel;

public class TheoryResponseModel
{
    public Guid Id { get; set; }

    public Guid LessonId { get; set; }

    public string TheoryName { get; set; } = null!;

    public string? TheoryDescription { get; set; }

    public string? TheoryContentJson { get; set; }

    public string? TheoryContentHtml { get; set; }

}