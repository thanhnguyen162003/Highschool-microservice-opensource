namespace Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Avatar { get; set; }
        public IEnumerable<Guid>? OwnerAvatar { get; set; }
        public int? TotalPlay { get; set; }
        public int? TotalHost { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Streak { get; set; }
        public IEnumerable<Guid>? HostKetIds { get; set; }
        public IEnumerable<Guid>? ParticipantKetIds { get; set; }

        public virtual ICollection<Ket> Kets { get; set; } = new HashSet<Ket>();
    }
}
