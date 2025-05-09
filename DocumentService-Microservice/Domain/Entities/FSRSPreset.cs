using System;

namespace Domain.Entities
{
    public class FSRSPreset
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public double[] FsrsParameters { get; set; } = new double[19];
        public double Retrievability { get; set; }
        public bool IsPublicPreset { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 