using System.ComponentModel.DataAnnotations;
using Application.Common.Models.FlashcardContentModel;

namespace Application.Common.Models.FlashcardModel;

public class FlashcardVoteModel
{
    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
    public double Star { get; set; } = 0;

}
