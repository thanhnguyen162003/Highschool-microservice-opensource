using Domain.Entities;
using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Domain.Entities.SqlEntites;

namespace Application.Common.Models.NewsModel;

public class NewsResponseModel
{
    public Guid Id { get; set; }
    public Author Author { get; set; }
    public string NewName { get; set; } = null!;
    public string? Content { get; set; }
    public string? ContentHtml { get; set; }
    public string? Image { get; set; }
    public string Slug { get; set; } = null!;
    public int? View { get; set; }
    public Guid NewsTagId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool Hot { get; set; }
    public bool IsDeleted { get; set; }
    public string? Location { get; set; }
    public string NewsTagName { get; set; } = null!;
    public List<Flashcard>? FlashcardList { get; set; }
    public List<Document>? DocumentList { get; set; }
    public List<Theory>? TheoryList { get; set; }
}
public class Flashcard
{
    public Guid FlashcardId { get; set; }
    public string FlashcardName { get; set; }
    public string FlashcardSlug { get; set; }
}
public class Document
{
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; }
    public string DocumentSlug { get; set; }
}
public class Theory
{
    public Guid TheoryId { get; set; }
    public string TheoryName { get; set; }
    //public string TheorySlug { get; set; }
}

