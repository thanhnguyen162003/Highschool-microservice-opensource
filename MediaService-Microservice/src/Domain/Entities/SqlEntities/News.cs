﻿using System.ComponentModel.DataAnnotations.Schema;
using Discussion_Microservice.Domain.Common;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;


namespace Domain.Entities.SqlEntites;
public class News : BaseAuditableEntitySQL
{
    [BsonRepresentation(BsonType.String)]
    public Guid NewsTagId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid AuthorId { get; set; }
    public string NewName { get; set; } = null!;
    public string? Content { get; set; }
    public string? ContentHtml { get; set; }
    public string Slug { get; set; } = null!;
    public string? Image { get; set; }
    public int? TotalView { get; set; }
    public int? TodayView { get; set; }
    public bool Hot { get; set; }
    public string? Location { get; set; }
    public bool IsDeleted { get; set; }
}
