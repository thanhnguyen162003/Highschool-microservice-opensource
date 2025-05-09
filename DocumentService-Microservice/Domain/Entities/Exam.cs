using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class Exam : BaseAuditableEntity
{
    public Guid Id { get; set; }

    public Guid SubjectId { get; set; }

    public string ExamName { get; set; } = null!;

    public string? ExamDescription { get; set; }

    public string ExamCode { get; set; } = null!;

    public int? View { get; set; }

    public string? Slug { get; set; }

    public string? ExamYear { get; set; }

    public string? Author { get; set; }

    public int? Grade { get; set; }

    public string? Type { get; set; }

    public int? Page { get; set; }

    public int? Download { get; set; }

    public int? Like { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ExamAnswer> ExamAnswers { get; set; } = new List<ExamAnswer>();

    public virtual Subject Subject { get; set; } = null!;
}
