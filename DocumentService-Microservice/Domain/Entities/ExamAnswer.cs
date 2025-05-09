using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class ExamAnswer : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public Guid ExamId { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Exam Exam { get; set; } = null!;
}
