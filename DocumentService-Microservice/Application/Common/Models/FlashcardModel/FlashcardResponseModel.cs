using Application.Common.Models.ContainerModel;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models.SearchModel;
using Domain.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardResponseModel : SearchResponseModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public FlashcardType? FlashcardType { get; set; } = Domain.Enums.FlashcardType.Lesson;
    public Guid? EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? EntitySlug { get; set; }

    public string? Grade { get; set; }

    public string FlashcardName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? FlashcardDescription { get; set; }

    public bool? Created { get; set; }

    public string Status { get; set; } = null!;

    public bool? IsRated { get; set; } = false!;

    public double Star { get; set; } = 0!;
    
    public string? CreatedBy { get; set; }
    
    public string? CreatedAt { get; set; }

    public string? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
    
    public int TodayView { get; set; } = 0!;
    
    public int TotalView { get; set; } = 0!;
    
    public int NumberOfFlashcardContent { get; set; }

	public ContainerResponseModel? Container { get; set; }
    

    public List<string>? Tags { get; set; } = new List<string>();
}