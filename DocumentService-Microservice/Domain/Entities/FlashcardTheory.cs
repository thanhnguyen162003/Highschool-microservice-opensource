using Domain.Common;

namespace Domain.Entities;

public class FlashcardTheory : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public string Term { get; set; } = null!;

    public string? Definition { get; set; }

    public string? TermRichText { get; set; }

    public string? DefinitionRichText { get; set; }

    public Guid TheoryId { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Theory Theory { get; set; } = null!;
}
