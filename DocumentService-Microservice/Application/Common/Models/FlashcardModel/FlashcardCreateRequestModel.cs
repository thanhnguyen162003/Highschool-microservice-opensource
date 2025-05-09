using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardCreateRequestModel
{
    [Required]
    public string FlashcardName { get; set; }
    [Required]
    public string FlashcardDescription { get; set; }
    public string? Status { get; set; }

    // Trường ID thống nhất
    [Required]
    public Guid EntityId { get; set; }
    [Required]
    public FlashcardType FlashcardType { get; set; } = Domain.Enums.FlashcardType.Lesson;

    // Danh sách các tag
    public List<string>? Tags { get; set; }
} 