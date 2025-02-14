using Application.Common.Models.ContainerModel;
using Application.Common.Models.FlashcardContentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardResponseModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public string? SubjectSlug { get; set; }

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

}