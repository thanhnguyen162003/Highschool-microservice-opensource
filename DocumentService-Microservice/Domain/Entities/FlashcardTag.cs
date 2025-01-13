using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities;

public class FlashcardTag
{
    public Guid Id { get; set; }

    public long FlashcardId { get; set; }

    public long TagId { get; set; }

    public DateTime? DeletedAt { get; set; }
}
