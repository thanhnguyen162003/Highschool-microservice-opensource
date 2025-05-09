using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class Theory : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public Guid LessonId { get; set; }

    public string TheoryName { get; set; } = null!;

    public string? TheoryDescription { get; set; }

    public string? TheoryContentJson { get; set; }

    public string? TheoryContentHtml { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;
}
