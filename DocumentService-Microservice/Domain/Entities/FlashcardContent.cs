using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class FlashcardContent : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public Guid FlashcardId { get; set; }

    public string FlashcardContentTerm { get; set; } = null!;

    public string? FlashcardContentDefinition { get; set; }
    
    public string? FlashcardContentDefinitionRichText { get; set; }
    
    public string? FlashcardContentTermRichText { get; set; }

    public required int Rank { get; set; }

    public string? Image { get; set; }

    public string? Status { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Flashcard Flashcard { get; set; } = null!;
    public virtual List<StarredTerm>? StarredTerm { get; set; } = new List<StarredTerm>();
	public virtual List<StudiableTerm>? StudiableTerm { get; set; } = new List<StudiableTerm>();
}
