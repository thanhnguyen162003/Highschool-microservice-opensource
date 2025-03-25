using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionalService.EntityFramework.Core.Common;

public abstract class BaseAuditableEntity
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}