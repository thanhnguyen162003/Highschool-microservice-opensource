namespace Application.Common.Models.SearchModel;

public class DocumentResponseModel
{
    public Guid Id { get; set; }
    public string? DocumentSlug { get; set; }

    public string DocumentName { get; set; } = null!;

    public string? DocumentDescription { get; set; }
    public int? DocumentYear { get; set; }

    public int? View { get; set; }
    public int? Download { get; set; }

    public int? Like { get; set; }
    public DocumentSubjectResponseModel Subject { get; set; }
    public string? Category { get; set; }

    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public DocumentHighlightResult? HighlightResult { get; set; }
}

public class DocumentSubjectResponseModel
{
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
    public string SubjectSlug { get; set; } = null!;
}

//public class DocumentCategoryResponseModel
//{
//    public Guid CategoryId { get; set; }
//    public string CategoryName { get; set; } = null!;
//    public string CategorySlug { get; set; } = null!;
//}

public class DocumentHighlightResult
{
    public DocumentHighlightField DocumentName { get; set; }
    public DocumentHighlightField DocumentDescription { get; set; }
}

public class DocumentHighlightField
{
    public string Value { get; set; }
    public string MatchLevel { get; set; }
    public bool FullyHighlighted { get; set; }
    public List<string> MatchedWords { get; set; }
}
