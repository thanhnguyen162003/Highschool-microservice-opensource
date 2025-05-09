using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Domain.QueriesFilter;
public class FlashcardQueryFilter
{
    public string? Search { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    [FromQuery(Name = "tags")]
    public string[]? Tags { get; set; }
    
    public Guid? EntityId { get; set; }
    
    public FlashcardType? FlashcardType { get; set; }
}

public class FlashcardQueryFilterManagement
{
    public string? Search { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    [FromQuery(Name = "tags")]
    public string[]? Tags { get; set; }

    public Guid? EntityId { get; set; }

    public FlashcardType? FlashcardType { get; set; }
    public Guid? UserId { get; set; }
    public bool? IsCreatedBySystem { get; set; }
    public string? Status { get; set; }
    public bool? IsDeleted { get; set; }
}